using System;
using System.Timers;

namespace Agridea.Prototypes.Akka.Common
{
    public class DoNothingJob : IJob
    {
        private readonly Random randomFailer_;
        private readonly Timer timer_;
        private int percentCounter_;
        private JobStatus status_;

        public DoNothingJob(string name)
        {
            Name = name;
            randomFailer_ = new Random();
            Status = JobStatus.Pending;
            var elapsedTime = new Random().Next(100, 1000);

            //Console.Write("Initializing job {0} that takes {1:##.0}s to complete.\n", Name, elapsedTime * 100.0 / 1000);
            timer_ = new Timer(elapsedTime) {AutoReset = true};
            timer_.Elapsed += (sender, eventargs) =>
            {
                var randomFailerValue = randomFailer_.Next(1, 300); // Fail 1 in 3.

                //Console.WriteLine($"job:{Name}, randomFailerValue={randomFailerValue}");
                var willFail = randomFailerValue == 1;
                if (willFail)
                {
                    timer_.Stop();
                    timer_.Close();
                    Status = JobStatus.Failed;
                }
                percentCounter_ += 1;
                if (percentCounter_ == 100)
                {
                    percentCounter_ = 0;
                    timer_.Stop();
                    timer_.Close();
                    Status = JobStatus.Completed;
                    Finished?.Invoke();
                }
                else
                {
                    Progressed?.Invoke(percentCounter_);
                }
            };
        }

        public string Name { get; }
        public JobStatus Status
        {
            get { return status_; }
            set
            {
                status_ = value;
                StatusChanged?.Invoke(status_);
            }
        }
        public event Action<int> Progressed;
        public event Action Finished;
        public event Action Started;
        public event Action<JobStatus> StatusChanged;

        public void Run()
        {
            //Console.WriteLine("Job \"" + Name + "\" starting.");
            timer_.Start();
            Started?.Invoke();
            Status = JobStatus.Running;
        }

        public void Cancel()
        {
            timer_.Stop();
            timer_.Close();
            Status = JobStatus.Canceled;
        }

        public void Pause()
        {
            timer_.Stop();
            Status = JobStatus.Paused;
        }

        public void Resume()
        {
            timer_.Start();
            Status = JobStatus.Running;
        }

        public string GetResult()
        {
            return $"Result for job {Name}";
        }
    }
}