using System.Collections.Generic;
using Agridea.Prototypes.Akka.Common;
using WebGrease.Css.Extensions;

namespace Agridea.Prototypes.Akka.Web
{
    public class JobViewModel
    {
        public string Name { get; set; }
        public int Percent { get; set; }
        public string Status { get; set; }
    }

    public static class JobViewModelExtensions
    {
        public static JobViewModel ToViewModel(this JobSummary job)
        {
            if (job == null)
                return null;

            return new JobViewModel {Name = job.Name, Percent = job.Percent, Status = job.Status.ToString() };
        }

        public static List<JobViewModel> ToViewModel(this IList<JobSummary> jobs)
        {
            if (jobs == null)
                return null;

            var list = new List<JobViewModel>(jobs.Count);
            jobs.ForEach(j => list.Add(j.ToViewModel()));
            return list;
        }
    }
}