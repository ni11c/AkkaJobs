using Akka.Actor;
using Akka.Event;

namespace Agridea.Prototypes.Akka.Common
{
    public class SipaAcotorV2 : ReceiveActor
    {
        private int jobCount_;
        private int numWaiting_;
        private readonly IActorRef supervisor_;

        public SipaAcotorV2(IActorRef supervisor)
        {
            supervisor_ = supervisor;
            var log = Context.GetLogger();

            Receive<string>(s =>
            {
                if (s != "export")
                    return;

                numWaiting_++;
                string name = "job" + ++jobCount_;
                var job = new DoNothingJob(name);

                job.Finished += () =>
                {
                    supervisor_.Tell(new Progress(name, 100));
                    supervisor_.Tell(new Finished(name));
                    
                    log.Info(name + " finished.");
                    numWaiting_--;
                    log.Info("num jobs in pipeline: " + numWaiting_);
                };

                job.Progressed += (percent) =>
                {
                    supervisor_.Tell(new Progress(name, percent));
                };

                job.Started += () =>
                {
                    supervisor_.Tell(new Started(name));
                };

                log.Info(name + " starting");
                log.Info("num jobs in pipeline: " + numWaiting_);
                job.Run();
            });
        }
    }
}
