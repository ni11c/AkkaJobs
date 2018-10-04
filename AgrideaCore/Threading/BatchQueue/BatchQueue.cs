using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Agridea.Timers;
using Timer = Agridea.Timers.Timer;

namespace Agridea.Threading
{
    /// <summary>
    /// 30.10.2014 :
    /// - Canceled is ambiguous : should be Running=>Interupted and Pending=>Canceled (even for this later case removed from queue?)
    /// -
    ///
    /// During cancellation check canceled if not throw an exception (looping treatment)
    ///
    /// Defines an active multithread safe batch queue with a dedicated thread for servicing jobs.
    /// This is in fact a job list with a scheduler which dispatches with a FIFO policy
    /// - Jobs can be added and canceled at anytime.
    /// - Jobs can be canceled all at a time or individually (based on a job's id)
    /// - Job treatements are serialized
    /// - Jobs can or cannot be duplicated (filtering based on a job's Id)
    /// The lifecycle of jobs added into queue falls in (a=Added, s=Started, f=Finished, c=Canceled) :
    /// - a;s;f (normal case)
    /// - a;c   (canceled before started)
    /// - a;s;c (canceled after started)
    /// If several jobs are added into queues, the possible traces are (e.g. 3 jobs) :
    /// - a1;a2;a3;s1;f1;s2;f2;s3;f3 (normal case)
    /// - a1;a2;a3;c1;c2;c3          (all jobs canceled immediately)
    /// - a1;a2;a3;s1;f1;s2;c2;c3    (jobs canceled after first finished and second started)
    /// Patrick's remark : while the FW class does not raise the Canceled events and prevent the job from running
    /// Other
    /// - in ExtractNextJob, see if there is a race between Monitor.Wait(this) and if(m_jobs.Count == 0)
    /// - block any attempt to add a non finished/cancelled job
    /// </summary>
    public sealed class BatchQueue : Disposable, IBatchQueue
    {
        #region Members

        private Thread thread_;
        private List<IJob> jobs_;
        private Timer timer_;

        #endregion Members

        #region Initialization

        public BatchQueue(string threadName, ThreadPriority threadPriority, int schedulingInterval = 60000)
        {
            timer_ = new Timer();
            timer_.Tick += OnTimerTick;
            timer_.Start(schedulingInterval);

            jobs_ = new List<IJob>();

            thread_ = new Thread(new ThreadStart(Dispatch));
            thread_.Name = threadName;
            thread_.Priority = threadPriority;
        }

        #endregion Initialization

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            Log.Verbose(string.Format("'{0}:{1}' Dispose()", GetType().FullName, GetHashCode()));
            if (disposing)
            {
                //Release managed resources.

                CancelAll();
                lock (this) Monitor.Pulse(this);
                thread_.Join(); //There should be no hang here, the thread is warned to finish (m_disposed set to 1, see Dispose())
            }
        }

        #endregion IDisposable

        #region Services

        public object SyncRoot { get { return this; } }

        public CultureInfo CurrentUICulture
        {
            get { return thread_.CurrentUICulture; }
            set { thread_.CurrentUICulture = value; }
        }

        public IJob[] Jobs
        {
            get { return jobs_.ToArray(); }
        }

        public void Start()
        {
            thread_.Start();
        }

        public void Clear()
        {
            CancelAll();
            lock (this)
            {
                jobs_.Clear();
            }
        }

        public void Add(IJob job)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);
            Log.Verbose("BatchQueue:Add({0})>", job.Code);

            lock (this)
            {
                Log.Verbose("BatchQueue:Add lock ({0}) >", job.Code);
                if (Contains(job))
                {
                    Log.Warning("Job {0} still in queue, running? {1}", job.Code, job.JobState);
                    return;
                }

                job.RequestDate = DateTime.Now;
                jobs_.Add(job);

                //Notifying while being locked is an issue since the duration of the treatment occuring upon notification
                //is not bound, but :
                //- SafeAdd and Monitor.Pulse(this) cannot be swapped
                //- Monitor.Pulse must be inside a lock
                //- lock cannot be split in three parts : lock(this)...; SafeAdd; lock(this)...
                SafeAdd(job);

                Monitor.Pulse(this);
                Log.Verbose("BatchQueue:Add lock ({0}) <", job.Code);
            }

            Log.Verbose("BatchQueue:Add({0})<", job.Code);
        }

        public void Remove(IJob job)
        {
            Log.Verbose("BatchQueue:Remove({0})>", job.Code);
            lock (this)
            {
                if (!Contains(job)) return;
                jobs_.Remove(job);
                Monitor.Pulse(this);
            }
            Log.Verbose("BatchQueue:Remove({0})<", job.Code);
        }

        public void Cancel(IJob job)
        {
            Log.Verbose("BatchQueue:Cancel({0})>", job.Code);
            Asserts<ArgumentNullException>.IsNotNull(job);

            lock (this)
            {
                //Needed only for requests sent from another process
                var jobInQueue = jobs_.Find(x => x.Equals(job));
                if (jobInQueue == null) return;
                if (jobInQueue.JobState == JobStates.Pending)
                    jobInQueue.JobState = JobStates.Canceled;
                else
                    jobInQueue.CancelRequested = true;
            }
            Log.Verbose("BatchQueue:Cancel({0})<", job.Code);
        }

        public void CancelPendingJobs()
        {
            lock (this) DoCancelJobs(false);
        }

        public void CancelAll()
        {
            lock (this) DoCancelJobs(true);
        }

        #endregion Services

        #region Event Handlers
        private void OnTimerTick(object sender, TimeEventArgs args)
        {
            lock (this)
            {
                Monitor.Pulse(this);
            }
        }
        #endregion

        #region Helpers

        private void Dispatch()
        {
            //The scheduling policy is :
            //FIFO and all job when looked at are runnable, there is no extra condition
            try
            {
                Log.Verbose("BatchQueue:Dispatch()>");
                while (!Disposed)
                {
                    IJob job = NextJobToSchedule();

                    if (job == null)
                    {
                        //Thread.Sleep(100); for experimenting with Active Objects (School/Users/Thierry)
                        continue;
                    }

                    SafeRun(job);

                    //If job finished properly and rescheduled itself for a later date
                    //add it back for this later date
                    if (job.RequestDate > DateTime.Now)
                    {
                        var requestDate = job.RequestDate;
                        Remove(job);
                        job.JobState = JobStates.Pending;
                        Add(job);
                        job.RequestDate = requestDate;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                Log.Verbose("BatchQueue:Dispatch()<");
            }
        }

        private bool Contains(IJob job)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);

            return jobs_.Contains(job);
        }

        private IJob NextJobToSchedule() //Schedule policy
        {
            try
            {
                Log.Verbose("BatchQueue:NextJobToSchedule()>");
                lock (this)
                {
                    var jobsReadyToRun = jobs_.Where(x => x.JobState == JobStates.Pending && x.RequestDate <= DateTime.Now);
                    var minimumPlannedDate = jobsReadyToRun.Any() ? jobsReadyToRun.Min(x => x.RequestDate) : DateTime.MinValue;
                    var nextJobToSchedule = jobsReadyToRun.FirstOrDefault(x => x.RequestDate == minimumPlannedDate);
                    if (nextJobToSchedule == null) Monitor.Wait(this);
                    if (nextJobToSchedule == null) return null;
                    return nextJobToSchedule;
                }
            }
            finally
            {
                Log.Verbose("BatchQueue:NextJobToSchedule()<");
            }
        }

        private void DoCancelJobs(bool includeRunning)
        {
            Log.Verbose("BatchQueue:DoCancelJobs()>");

            foreach (IJob candidateJobToCancel in jobs_)
            {
                if (!includeRunning && candidateJobToCancel.JobState == JobStates.Running) continue;
                Cancel(candidateJobToCancel);
            }

            Log.Verbose("BatchQueue:DoCancelJobs()<");
        }

        private void SafeRun(IJob job)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);
            Log.Verbose("BatchQueue:SafeRun({0})>", job.Code);
            try
            {
                SafeStart(job);
                job.Run();
                SafeFinish(job);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                Log.Warning("Job {0} {1}", job.Code, operationCanceledException.Message);
                SafeCancel(job);
            }
            catch (Exception exception)
            {
                Log.Error("Job {0} {1}", job.Code, exception.Message);
                SafeFail(job, exception);
            }
            finally
            {
                Log.Verbose("BatchQueue:SafeRun({0})<", job.Code);
            }
        }

        private void SafeAdd(IJob job)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);
            Log.Verbose("BatchQueue:SafeAdd({0})>", job.Code);

            try
            {
                job.Add();
            }
            catch (Exception e)
            {
                Log.Error("Job {0} {1}", job.Code, e.Message);
            }
            finally
            {
                Log.Verbose("BatchQueue:SafeAdd({0})<", job.Code);
            }
        }

        private void SafeStart(IJob job)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);
            Log.Verbose("BatchQueue:SafeStart({0})>", job.Code);

            try
            {
                job.Start();
            }
            catch (Exception e)
            {
                Log.Error("Job {0} {1}", job.Code, e.Message);
            }
            finally
            {
                Log.Verbose("BatchQueue:SafeStart({0})<", job.Code);
            }
        }

        private void SafeFinish(IJob job)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);
            Log.Verbose("BatchQueue:SafeFinish({0})>", job.Code);

            try
            {
                job.Finish();
            }
            catch (Exception e)
            {
                Log.Error("Job {0} {1}", job.Code, e.Message);
            }
            finally
            {
                Log.Verbose("BatchQueue:SafeFinish({0})<", job.Code);
            }
        }

        private void SafeCancel(IJob job)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);
            Log.Verbose("BatchQueue:SafeCancel({0})>", job.Code);

            try
            {
                job.Cancel();
            }
            catch (Exception e)
            {
                Log.Error("Job {0} {1}", job.Code, e.Message);
            }
            finally
            {
                Log.Verbose("BatchQueue:SafeCancel({0})<", job.Code);
            }
        }

        private void SafeFail(IJob job, Exception exception)
        {
            Asserts<ArgumentNullException>.IsNotNull(job);
            Log.Verbose("BatchQueue:SafeFail({0})>", job.Code);

            try
            {
                job.Fail(exception);
            }
            catch (Exception e)
            {
                Log.Error("Job {0} {1}", job.Code, e.Message);
            }
            finally
            {
                Log.Verbose("BatchQueue:SafeFail({0})<", job.Code);
            }
        }

        #endregion Helpers
    }
}