using Topshelf;

namespace Agridea.Prototypes.Akka.Remote
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Remote4Web>();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.UseAssemblyInfoForServiceInfo();
                x.EnableServiceRecovery(r => r.RestartService(1));

                x.SetDescription("Worker and supervisor actors using Akka.net");
                x.SetDisplayName("Agridea.Prototypes.Akka.Remote4Web");
                x.SetServiceName("Agridea.Prototypes.Akka.Remote4Web");
            });
        }
    }
}
