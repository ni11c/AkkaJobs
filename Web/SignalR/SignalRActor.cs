using System.Collections.Generic;
using Agridea.Prototypes.Akka.Common;
using Agridea.Prototypes.Akka.Common.Messages;
using Agridea.Prototypes.Akka.Web;
using Akka.Actor;
using Akka.Event;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Web.SignalR
{
    public class SignalRActor : ReceiveActor
    {
        private readonly ILoggingAdapter log_ = Context.GetLogger();
        protected TheHub Hub { get; private set; }

        public SignalRActor()
        {
            #region Commands

            Receive<string>(s =>
            {
                log_.Info("Command received: " + s);
                switch (s)
                {
                    case "getjobs":
                        Actors.Supervisor.Tell("getjobs");
                        break;

                    case "start":
                        Actors.Supervisor.Tell("start");
                        break;

                    case "register":
                        Actors.Supervisor.Tell(new SupervisorActorV2.Hello(Actors.Guid));
                        break;

                    case "unregister":
                        Actors.Supervisor.Tell(new SupervisorActorV2.Bye(Actors.Guid));
                        break;

                    case "purge":
                        Actors.Supervisor.Tell("purge");
                        break;

                    case "purged": // is feedback actually
                        Actors.Supervisor.Tell("getjobs");
                        break;
                }
            });

            Receive<Cancel>(cancel => Actors.Supervisor.Tell(cancel));
            Receive<Pause>(pause => Actors.Supervisor.Tell(pause));
            Receive<Resume>(resume => Actors.Supervisor.Tell(resume));
            Receive<GetResult>(getResult => Actors.Supervisor.Tell(getResult));

            #endregion

            #region Feedback

            Receive<RegistrationSuccessful>(msg => { Hub.WriteMessage("Connected to jobs queue.\n"); });

            Receive<JobSummary>(job =>
            {
                Hub.WriteMessage("Job " + job.Name + " is " + job.Percent + "% complete.");
                Hub.UpdateJob(job);
            });

            Receive<Finished>(finished => { Hub.WriteMessage("Job " + finished.Name + " has completed."); });

            Receive<StatusChanged>(statusChanged =>
            {
                Hub.WriteMessage($"Job {statusChanged.Name} changed its status to {statusChanged.Status.ToString()}");
                Hub.UpdateJobStatus(new JobSummary {Name = statusChanged.Name, Status = statusChanged.Status});
            });

            Receive<IList<JobSummary>>(jobs => { Hub.DisplayJobs(jobs); });

            Receive<Result>(result =>
            {
                Hub.WriteMessage($"Received result {result.JobResult} from job {result.JobName}.");
                Hub.ShowResult(result);
            });

            #endregion

            log_.Info("SignalR actor constructed.");
        }

        protected override void PreStart()
        {
            log_.Info("SignalR hib starting...");
            var hubManager = new DefaultHubManager(GlobalHost.DependencyResolver);
            Hub = hubManager.ResolveHub("theHub") as TheHub;
            log_.Info("SignalR hub started.");
            log_.Info("SignalR actor started.");
        }
    }
}