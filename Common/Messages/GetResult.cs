namespace Agridea.Prototypes.Akka.Common
{
    public class GetResult
    {
        public string JobName { get; }

        public string ConsumerId { get; }

        public string ClientId { get; }

        public GetResult(string jobName, string consumerId, string clientId)
        {
            JobName = jobName;
            ConsumerId = consumerId;
            ClientId = clientId;
        }
    }
}
