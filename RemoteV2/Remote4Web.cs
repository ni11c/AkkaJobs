using Agridea.Prototypes.Akka.Common;
using Akka.Actor;
using Topshelf;

namespace Agridea.Prototypes.Akka.Remote
{
    public class Remote4Web : ServiceControl
    {
        private ActorSystem system_;

        public bool Start(HostControl hostControl)
        {
            system_ = ActorSystem.Create(ActorPaths.RemoteV2System.Name);
            system_.ActorOf(Props.Create(() => new SupervisorActorV2()), ActorPaths.Supervisor.Name);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            system_.Terminate();
            return true;
        }
    }

}
