using Agridea.Diagnostics.Contracts;

namespace System
{
    /// <summary>
    /// Represents a period of time consisting of two non overlapping contiguous periods.
    /// </summary>
    /// <remarks>
    /// DateTimePeriods is immutable (hence thread-safe).
    /// </remarks>
    public class NonContiguousDateTimePeriod
    {
        public DateTimePeriod FirstPeriod { get; private set; }
        public DateTimePeriod SecondPeriod { get; private set; }
        public DateTime EndDate
        {
            get { return SecondPeriod.EndDate; }
        }
        public DateTime StartDate
        {
            get { return FirstPeriod.StartDate; }
        }

        public NonContiguousDateTimePeriod(DateTimePeriod firstPeriod, DateTimePeriod secondPeriod)
        {
            Requires<ArgumentException>.IsTrue(secondPeriod.StartDate > firstPeriod.EndDate, "Second period must be posterior to first period.");
            FirstPeriod = firstPeriod;
            SecondPeriod = secondPeriod;
        }

        public bool Contains(DateTime dt)
        {
            return FirstPeriod.Contains(dt) || SecondPeriod.Contains(dt);
        }

        public NonContiguousDateTimePeriod AddYears(int years)
        {
            return new NonContiguousDateTimePeriod(FirstPeriod.AddYears(years), SecondPeriod.AddYears(years));
        }
    }

    public static partial class DateTimePeriodExtensions
    {
        public static bool In(this DateTime dt, NonContiguousDateTimePeriod period)
        {
            return period.Contains(dt);
        }
    }
}