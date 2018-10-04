using System;
using Agridea.DataRepository;

namespace Agridea.Calendar
{
    public partial class CalendarEventRecurrence : ICombo
    {
        #region Services
        public static readonly int Unique = 1;
        public static readonly int Daily = 2;
        public static readonly int Weekly = 3;
        public static readonly int Monthly = 4;
        public static readonly int TriMonthly = 5;
        public static readonly int Semesterly = 6;
        public static readonly int Yearly = 7;
        #endregion

        #region Implementation of ICombo
        public string ComboText
        {
            get { return Description; }
        }
        public Func<IPocoBase, object> SortFunc { get
            {
                return m => Code;
            } }
        #endregion
    }
}
