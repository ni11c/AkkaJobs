namespace Agridea.Prototypes.Akka.Common
{
    public class DoNothingJobFactory : IJobFactory
    {
        private static int sequence_;

        public IJob CreateJob()
        {
            var name = "Job";
            lock (this)
            {
                name += (++sequence_).ToString("0000");
            }

            return new DoNothingJob(name);
        }
    }
}