using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Agridea.Web.Mvc.Grid;

namespace Agridea.Calendar
{
    
    public class SearchCalendarEvent
    {
        public SearchCalendarEventInput Search { get; set; }
        public Grid<CalendarEventDisplayItem> Grid { get; set; }
        public IEnumerable<SelectListItem> ComboRecurrence { get; set; }
        public IEnumerable<SelectListItem> ComboStatus { get; set; }
    }

    public class SearchCalendarEventInput
    {
        public virtual string AppliesTo { get; set; }
        public virtual string Comment { get; set; }
        public virtual string Description { get; set; }
        public virtual Nullable<System.DateTime> EndDate { get; set; }
        public virtual string Owner { get; set; }
        public virtual Nullable<System.DateTime> StartDate { get; set; }
        public virtual string Title { get; set; }
        public int RecurrenceId { get; set; }
        public int StatusId { get; set; }
    }
}
