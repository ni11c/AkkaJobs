using System;
using Agridea.DataRepository;

namespace Agridea.Calendar
{
    public partial class CalendarEventStatus : ICombo
    {
        #region Services
        public static readonly int Defined = 1;
        public static readonly int OnGoing = 2;
        public static readonly int Cancelled = 3;
        public static readonly int Done = 4;
        public static readonly int Postponed = 5;
        #endregion

        #region Implementation of ICombo
        public string ComboText { get { return Description; } }
        public Func<IPocoBase, object> SortFunc { get { return m => Code; } }
        #endregion
    }
}
