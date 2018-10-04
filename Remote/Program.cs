using Topshelf;

namespace Agridea.Prototypes.Akka.Remote
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<SystemService>();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.UseAssemblyInfoForServiceInfo();
                x.EnableServiceRecovery(r => r.RestartService(1));

                x.SetDescription("actor system prototype using Akka.net");
                x.SetDisplayName("Agridea.Prototypes.Akka.Remote");
                x.SetServiceName("Agridea.Prototypes.Akka.Remote");
            });
        }
    }
}
