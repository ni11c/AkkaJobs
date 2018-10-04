using Agridea.Prototypes.Akka.Common;
using Akka.Actor;
using Topshelf;

namespace Agridea.Prototypes.Akka.Remote
{
    public class Supervisor : ServiceControl
    {
        private ActorSystem system_;

        public bool Start(HostControl hostControl)
        {
            system_ = ActorSystem.Create("supervisor");
            system_.ActorOf(Props.Create(() => new SupervisorActorV2());
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            system_.Terminate();
            return true;
        }
    }

}
