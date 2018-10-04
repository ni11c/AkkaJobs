using System.Collections.Generic;
using System.Linq;
using Agridea.Prototypes.Akka.Common.Messages;
using Akka.Actor;
using Akka.Util.Internal;

namespace Agridea.Prototypes.Akka.Common
{
    public class SupervisorActorV2 : ReceiveActor
    {
        private readonly IActorRef worker_ = Context.ActorOf(Props.Create(() => new JobActor(Context.Self, new DoNothingJobFactory())));
        //private readonly IActorRef worker_ = Context.ActorOf(Props.Create(() => new JobActor(Context.Self, new AgisJobFactory())));
        private readonly Dictionary<string, IActorRef> consumers_ = new Dictionary<string, IActorRef>();
        private readonly Dictionary<string, JobSummary> jobs_ = new Dictionary<string, JobSummary>();
        
        #region Messages
        public class Hello
        {
            public string Id { get; }

            public Hello(string id)
            {
                Id = id;
            }
        }

        public class Bye
        {
            public string Id { get; }

            public Bye(string id)
            {
                Id = id;
            }
        }
        #endregion

        /// <summary>
        /// The supervisor actor talks with signalR actor. It maintains a list of consumers, and then signalr as one of
        /// these consumer registers himself to supervisor it (supervisor.Tell("hello I am signalr")). It then tells all consumers
        /// about jobs progress. Signalr can also unregister, by telling supervisor, who removes him from his list of consumers.
        /// SignalR actor must know about supervisor when he gets created.
        /// </summary>
        public SupervisorActorV2()
        {
            #region Commands

            Receive<Hello>(hello =>
            {
                if (!consumers_.ContainsKey(hello.Id))
                    consumers_.Add(hello.Id, Sender);
                Sender.Tell(new RegistrationSuccessful());
            });

            Receive<Bye>(bye =>
            {
                if (consumers_.ContainsKey(bye.Id))
                    consumers_.Remove(bye.Id);
            });

            Receive<string>(s =>
            {
                switch (s)
                {
                    case "start":
                        worker_.Tell(new Run());
                        break;

                    case "purge":
                        PurgeFinishedJobs();
                        Sender.Tell("purged");
                        break;

                    case "getjobs":
                        Sender.Tell(jobs_.Values.ToList());
                        break;
                }
            });

            Receive<Cancel>(cancel => worker_.Tell(cancel));
            Receive<Pause>(pause => worker_.Tell(pause));
            Receive<Resume>(resume => worker_.Tell(resume));
            Receive<GetResult>(getResult => worker_.Tell(getResult));

            #endregion

            #region Feedback

            Receive<Started>(started =>
            {
                var job = new JobSummary { Name = started.Name, Percent = 0, Status = JobStatus.Running };
                jobs_.Add(job.Name, job);
                consumers_.ForEach(c => c.Value.Tell(job));
            });

            Receive<Progress>(progress =>
            {
                JobSummary job;
                jobs_.TryGetValue(progress.Name, out job);
                if (job != null)
                {
                    job.Percent = progress.Percent;
                    consumers_.ForEach(c => c.Value.Tell(job));
                }
            });

            Receive<Finished>(finished =>
            {
                consumers_.ForEach(c => c.Value.Tell(finished));
            });

            Receive<StatusChanged>(statusChanged =>
            {
                JobSummary job;
                jobs_.TryGetValue(statusChanged.Name, out job);
                if (job != null)
                {
                    job.Status = statusChanged.Status;
                    consumers_.ForEach(c => c.Value.Tell(statusChanged));
                }
            });

            Receive<Result>(result =>
            {
                IActorRef consumer;
                consumers_.TryGetValue(result.ConsumerId, out consumer);
                consumer?.Tell(result);
            });

            #endregion
        }

        private void PurgeFinishedJobs()
        {
            var keysToRemove = jobs_.Where(j => j.Value.Status == JobStatus.Completed).Select(j => j.Key).ToList();
            keysToRemove.ForEach(key => jobs_.Remove(key));
        }
    }

    public class JobSummary
    {
        public string Name { get; set; }
        public int Percent { get; set; }
        public JobStatus Status { get; set; }
    }
}
