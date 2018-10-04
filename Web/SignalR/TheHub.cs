using System.Collections.Generic;
using Agridea.Prototypes.Akka.Common;
using Agridea.Prototypes.Akka.Web;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Web.SignalR
{
    [HubName("theHub")]
    public class TheHub : Hub
    {
        public void Register()
        {
            Actors.SignalR.Tell("register", ActorRefs.Nobody);
        }

        public void StartJob()
        {
            Actors.SignalR.Tell("start", ActorRefs.Nobody);       
        }

        public void GetJobs()
        {
            Actors.SignalR.Tell("getjobs", ActorRefs.Nobody);
        }

        public void Purge()
        {
            Actors.SignalR.Tell("purge", ActorRefs.Nobody);
        }

        public void Cancel(string name)
        {
            Actors.SignalR.Tell(new Cancel(name));
        }

        public void Pause(string name)
        {
            Actors.SignalR.Tell(new Pause(name));
        }

        public void Resume(string name)
        {
            Actors.SignalR.Tell(new Resume(name));
        }

        public void GetResult(string jobName, string clientId)
        {
            Actors.SignalR.Tell(new GetResult(jobName, Actors.Guid, clientId));
        }

        public void WriteMessage(string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TheHub>();
            context.Clients.All.printMessage(message);
        }

        public void DisplayJobs(IList<JobSummary> jobs)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TheHub>();
            context.Clients.All.displayJobs(jobs.ToViewModel());
        }

        public void UpdateJob(JobSummary job)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TheHub>();
            context.Clients.All.updateJob(job.ToViewModel());
        }

        public void UpdateJobStatus(JobSummary job)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TheHub>();
            context.Clients.All.updateJobStatus(job.ToViewModel());
        }

        public void ShowResult(Result result)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TheHub>();
            context.Clients.All.showResult(result);
        }
    }
}