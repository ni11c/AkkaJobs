using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace Agridea.Prototypes.Akka.Common
{
    public class JobActor : ReceiveActor
    {
        private readonly IActorRef supervisor_;
        private readonly Dictionary<string, IJob> jobs_; //assume unique job names 

        public JobActor(IActorRef supervisor, IJobFactory jobFactory)
        {
            supervisor_ = supervisor;
            jobs_ = new Dictionary<string, IJob>();
            var log = Context.GetLogger();

            Receive<Run>(run =>
            {
                var job = jobFactory.CreateJob();
                job.Started += () =>
                {
                    supervisor_.Tell(new Started(job.Name));
                    log.Info($"Job {job.Name} started.");
                };
                job.Progressed += (percent) =>
                {
                    supervisor_.Tell(new Progress(job.Name, percent));
                    log.Debug($"Job {job.Name} is now {percent}% complete.");
                };
                job.StatusChanged += status =>
                {
                    supervisor_.Tell(new StatusChanged(job.Name, job.Status));
                    log.Info($"Job {job.Name} has changed status to {job.Status.ToString()}.");
                };
                job.Finished += () =>
                {
                    supervisor_.Tell(new Progress(job.Name, 100));
                    supervisor_.Tell(new Finished(job.Name));
                    log.Info($"Job {job.Name} finished.");
                };

                log.Info($"Job {job.Name} starting.");
                jobs_.Add(job.Name, job);
                job.Run();
            });

            Receive<Cancel>(cancel =>
            {
                IJob job;
                jobs_.TryGetValue(cancel.Name, out job);
                log.Info($"Canceling job {cancel.Name}.");
                job?.Cancel();
            });

            Receive<Pause>(pause =>
            {
                IJob job;
                jobs_.TryGetValue(pause.Name, out job);
                if (job != null && job.Status == JobStatus.Running)
                {
                    log.Info($"Pausing job {pause.Name}.");
                    job.Pause();
                }
            });

            Receive<Resume>(resume =>
            {
                IJob job;
                jobs_.TryGetValue(resume.Name, out job);
                if (job != null && job.Status == JobStatus.Paused)
                {
                    log.Info($"Resuming job {resume.Name}.");
                    job.Resume();
                }
            });

            Receive<GetResult>(getResult =>
            {
                IJob job;
                jobs_.TryGetValue(getResult.JobName, out job);
                if (job != null && job.Status == JobStatus.Completed)
                {
                    log.Info($"getting result for job {getResult.JobName}.");
                    var result = job.GetResult();
                    supervisor_.Tell(new Result(job.Name, result, getResult.ConsumerId, getResult.ClientId));
                }
            });
        }
    }
}
