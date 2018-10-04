namespace Agridea.Prototypes.Akka.Common
{
    public class Result
    {
        public string JobName { get; }
        public string JobResult { get; }
        public string ConsumerId { get; }
        public string ClientId { get; }

        public Result(string jobName, string result, string consumerId, string clientId)
        {
            JobName = jobName;
            JobResult = result;
            ConsumerId = consumerId;
            ClientId = clientId;
        }
    }
}