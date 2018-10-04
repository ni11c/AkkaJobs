using Agridea.Acorda.Agis;
using Agridea.Prototypes.Akka.Common.Providers;

namespace Agridea.Prototypes.Akka.Common
{
    public class AgisJobFactory : IJobFactory
    {
        private static int sequence_;

        public IJob CreateJob()
        {
            var name = "Job";
            lock (this)
            {
                name += (++sequence_).ToString("0000");
            }

            return new AgisJob(name, new StructureExporter(new WebApiDataProvider(), new InMemoryStorageProvider(), ExportContext.Default));
        }
    }
}