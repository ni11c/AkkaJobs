using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using System;

namespace Agridea.Threading
{
    public abstract class JobBase
    {
        #region Constants
        #endregion

        #region Members
        private string progressRate_;
        #endregion

        #region Initialization
        protected JobBase()
        {
            Reset();
        }

        protected JobBase(string id)
            : this()
        {
            Asserts<ArgumentNullException>.IsNotEmpty(id);
            Code = id;
            JobState = JobStates.Pending;
        }
        #endregion

        #region Object
        public override string ToString() { return string.Format("[{0}: Id={0}]", GetType().Name); }
        public override int GetHashCode() { return Code.GetHashCode(); }
        public sealed override bool Equals(object obj) { return Equals(obj as IJob); }
        #endregion

        #region IEquatable<IJob>
        public bool Equals(IJob other)
        {
            if (ReferenceEquals(other, null)) return false;
            return ReferenceEquals(other, this) || Code.Equals(other.Code);
        }
        #endregion

        #region ICancelable
        public bool CancelRequested { get; set; }
        #endregion

        #region IJob
        public string Code { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestedBy { get; set; }
        public string Input { get; set; }
        public string[] Parameters { get; set; }
        public string ProgressRate
        {
            get
            {
                return progressRate_;
            }
            set
            {
                progressRate_ = value;
                RaiseProgress(new JobProgressEventArgs(progressRate_));
            }
        }
        public JobStates JobState { get; set; }
        public string TimeElapsed { get; set; }
        public int WarningCount { get; set; }
        public int ErrorCount { get; set; }
        public string Result { get; set; }
        public string Diagnostics { get; set; }

        public void Reset()
        {
            lock (this)
            {
                CancelRequested = false;
                JobState = JobStates.Pending;
                ProgressRate = string.Empty;
                WarningCount = 0;
                ErrorCount = 0;
                Result = "n/a";
                Diagnostics = "";
            }
        }
        public void Add()
        {
            Log.Verbose("JobBase:Add({0})> state {1}", Code, JobState);
            RaiseAdded();
            Log.Verbose("JobBase:Add({0})< state {1}", Code, JobState);
        }
        public void Start()
        {
            Log.Verbose("JobBase:Start({0})> state {1}", Code, JobState);
            Asserts<InvalidOperationException>.IsFalse(JobState == JobStates.Running, string.Format("Incorrect running {0}", JobState));

            lock (this)
            {
                JobState = JobStates.Running;
            }
            RaiseStarted();

            Log.Verbose("JobBase:Start({0})< running {1}", Code, JobState);
        }
        public abstract void Run();
        public void Cancel()
        {
            Log.Verbose("JobBase:Cancel({0})> state {1}", Code, JobState);
            Asserts<InvalidOperationException>.IsTrue(JobState == JobStates.Running, string.Format("Incorrect running {0}", JobState));

            lock (this)
            {
                JobState = JobStates.Canceled;
            }
            RaiseCanceled(new JobCanceledEventArgs());

            Log.Verbose("JobBase:Cancel({0})< state {1}", Code, JobState);
        }
        public void Fail(Exception e)
        {
            Asserts<ArgumentNullException>.IsNotNull(e);
            Log.Verbose("JobBase:Fail({0})> state {1}", Code, JobState);

            lock (this)
            {
                JobState = JobStates.Failed;
            }
            RaiseFailed(new JobFailedEventArgs(e));

            Log.Verbose("JobBase:Fail({0})< state {1}", Code, JobState);
        }
        public void Finish()
        {
            Log.Verbose("JobBase:Finish({0})> state {1}", Code, JobState);
            Asserts<InvalidOperationException>.IsTrue(JobState == JobStates.Running, string.Format("Incorrect running {0}", JobState));

            lock (this)
            {
                JobState = JobStates.Finished;
            }
            RaiseFinished();

            Log.Verbose("JobBase:Finish({0})< state {1}", Code, JobState);
        }

        public event EventHandler Added;
        public event EventHandler Started;
        public event OperationProgressEventHandler Progress;
        public event EventHandler Finished;
        public event JobCanceledEventHandler Canceled;
        public event JobFailedEventHandler Failed;
        #endregion

        #region Helpers
        private void RaiseAdded()
        {
            if (Added == null) return;
            Added(this, EventArgs.Empty);
        }
        private void RaiseStarted()
        {
            if (Started == null) return;
            Started(this, EventArgs.Empty);
        }
        private void RaiseProgress(JobProgressEventArgs e)
        {
            if (Progress == null) return;
            Progress(this, e);
        }
        private void RaiseFinished()
        {
            if (Finished == null) return;
            Finished(this, EventArgs.Empty);
        }
        private void RaiseCanceled(JobCanceledEventArgs e)
        {
            if (Canceled == null) return;
            Canceled(this, e);
        }
        private void RaiseFailed(JobFailedEventArgs e)
        {
            if (Failed == null) return;
            Failed(this, e);
        }
        #endregion
    }
}
