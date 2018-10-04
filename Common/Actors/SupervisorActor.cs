using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace Agridea.Prototypes.Akka.Common
{
    public class SupervisorActor : ReceiveActor
    {
        private readonly IActorRef sipaActor_;
        private readonly List<Progress> jobs_;
        private readonly IJobsDisplayer<List<Progress>>  displayer_;
        private readonly ILoggingAdapter log_ = Context.GetLogger();

        public SupervisorActor(IActorRef sipaActor, IJobsDisplayer<List<Progress>> displayer)
        {
            sipaActor_ = sipaActor;
            jobs_ = new List<Progress>();
            displayer_ = displayer;

            #region Commands
            Receive<string>(s =>
            {
                if (s != "start")
                    return;

                sipaActor_.Tell("export");
            });

            Receive<Cancel>(cancel =>
            {
                //sipaActor_.Tell(cancel);
            });
            #endregion

            #region Feedback

            Receive<Started>(started =>
            {
                jobs_.Add(new Progress(started.Name, 0));
                log_.Info("New job started, name=" + started.Name);
            });

            Receive<Progress>(progress =>
            {
                jobs_[jobs_.FindIndex(prog => prog.Name == progress.Name)].Percent = progress.Percent;
                displayer_.Display(jobs_);
                log_.Info("Job " + progress.Name + " is " + progress.Percent + "% completed.");
            });

            Receive<Finished>(finished =>
            {
                // to keep it simple, jobs_ keeps all jobs including those finished.
                log_.Info("Job " + finished.Name + " has finished.");
            });

            #endregion
        }
    }
}
