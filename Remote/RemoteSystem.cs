using Akka.Actor;
using Topshelf;

namespace Agridea.Prototypes.Akka.Remote
{
    public class SystemService : ServiceControl
    {
        private ActorSystem system_;

        public bool Start(HostControl hostControl)
        {
            system_ = ActorSystem.Create("SipaServer");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            system_.Terminate();
            return true;
        }
    }

}
