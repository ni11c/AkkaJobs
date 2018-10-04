using System.Linq;
using Agridea.Diagnostics.Contracts;

namespace System
{
    /// <summary>
    /// Represents a period of continuous time between two dates.
    /// </summary>
    /// <remarks>
    /// DateTimePeriod is immutable
    /// </remarks>
    public struct DateTimePeriod
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public DateTimePeriod(DateTime start, DateTime end) : this()
        {
            Requires<ArgumentOutOfRangeException>.IsTrue(start < end, "Start date must be earlier than End date in DateTimePeriod.");
            StartDate = start;
            EndDate = end;
        }

        public DateTimePeriod OuterMerge(DateTimePeriod dtp)
        {
            return new DateTimePeriod(
                dtp.StartDate < StartDate ? dtp.StartDate : StartDate,
                dtp.EndDate > EndDate ? dtp.EndDate : EndDate
                );
        }

        public static DateTimePeriod? OuterMerge(params DateTimePeriod[] dtp)
        {
            if (!dtp.Any())
                return null;

            return new DateTimePeriod(
                dtp.Min(x => x.StartDate),
                dtp.Max(x => x.EndDate)
                );
        }

        public DateTimePeriod InnerMerge(DateTimePeriod dtp)
        {
            return new DateTimePeriod(
                dtp.StartDate < StartDate ? StartDate : dtp.StartDate,
                dtp.EndDate > EndDate ? EndDate : dtp.EndDate
                );
        }

        public static DateTimePeriod? InnerMerge(params DateTimePeriod[] dtp)
        {
            if (!dtp.Any())
                return null;

            return new DateTimePeriod(
                dtp.Max(x => x.StartDate),
                dtp.Min(x => x.EndDate)
                );
        }

        public bool Contains(DateTime dt)
        {
            return StartDate <= dt && dt < EndDate;
        }

        public DateTimePeriod AddYears(int years)
        {
            return new DateTimePeriod(StartDate.AddYears(years), EndDate.AddYears(years));
        }
    }

    public static partial class DateTimePeriodExtensions
    {
        public static bool In(this DateTime dt, DateTimePeriod period)
        {
            return period.Contains(dt);
        }
    }
}