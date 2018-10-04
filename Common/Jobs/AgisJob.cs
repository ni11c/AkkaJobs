using System;
using System.IO;
using Agridea.Acorda.Agis;

namespace Agridea.Prototypes.Akka.Common
{
    public class AgisJob : IJob
    {
        private StructureExporter exporter_;
        private JobStatus status_;

        public AgisJob(string name, StructureExporter exporter)
        {
            Name = name;
            exporter_ = exporter;
            Status = JobStatus.Pending;
        }

        public event Action<int> Progressed;
        public event Action Finished;
        public event Action Started;
        public event Action<JobStatus> StatusChanged;
        public string Name { get; }
        public JobStatus Status
        {
            get { return status_; }
            private set
            {
                status_ = value;
                StatusChanged?.Invoke(status_);
            }
        }

        public void Run()
        {
            DoWork();
            Started?.Invoke();
            Status = JobStatus.Running;
        }

        public string GetResult()
        {
            return $"Result for job {Name}";
        }

        public void Pause()
        {
            Status = JobStatus.Paused;
        }

        public void Resume()
        {
            Status = JobStatus.Running;
        }

        public void Cancel()
        {
            Status = JobStatus.Canceled;
        }

        #region Helpers

        private void DoWork()
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string timestamp = Extensions.TimeStampNow();
            string zipFile = Path.Combine(desktopPath, "data_" + timestamp + ".zip");
            string envlFile = Path.Combine(desktopPath, "envl_" + timestamp + ".zip");

            using (FileStream zipStream = File.Create(zipFile))
            using (FileStream envlStream = File.Create(envlFile))
                exporter_.ExportAsync(50, zipStream, envlStream);
        }

        #endregion
    }
}