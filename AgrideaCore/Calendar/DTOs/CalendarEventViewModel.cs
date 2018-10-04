using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agridea
{
    public class CalendarEventDisplayItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public string AppliesTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RecurrenceDescription { get; set; }
        public string StatusDescription { get; set; }
        public string Comment { get; set; }
        public string FileName { get; set; }
    }

    public class CalendarEventEdit : IHasId
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public string AppliesTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RecurrenceId { get; set; }
        public int StatusId { get; set; }
        public string Comment { get; set; }
        public HttpPostedFileBase Attachment { get; set; }

        public IEnumerable<SelectListItem> ComboRecurrence { get; set; }
        public IEnumerable<SelectListItem> ComboStatus { get; set; }
    }
}