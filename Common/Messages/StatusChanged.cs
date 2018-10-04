namespace Agridea.Prototypes.Akka.Common
{
    public class StatusChanged
    {
        public string Name { get; }

        public JobStatus Status { get; }

        public StatusChanged(string name, JobStatus status)
        {
            Name = name;
            Status = status;
        }
    }
}
