namespace System
{
    /// <remarks>
    /// Truncate code thanks to http://stackoverflow.com/questions/1004698/how-to-truncate-milliseconds-off-of-a-net-datetime
    /// </remarks>
    public static class DateTimeExtensions
    {
        #region Truncate methods

        public static DateTime TruncateToMillisecond(this DateTime dt)
        {
            return dt.TruncateTo(TimeSpan.TicksPerMillisecond);
        }

        public static DateTime TruncateToHundredthOfSecond(this DateTime dt)
        {
            return dt.TruncateTo(TimeSpan.TicksPerMillisecond * 10);
        }

        public static DateTime TruncateToSecond(this DateTime dt)
        {
            return dt.TruncateTo(TimeSpan.TicksPerSecond);
        }

        public static DateTime TruncateToMinute(this DateTime dt)
        {
            return dt.TruncateTo(TimeSpan.TicksPerMinute);
        }

        public static DateTime TruncateToHour(this DateTime dt)
        {
            return dt.TruncateTo(TimeSpan.TicksPerHour);
        }

        public static DateTime TruncateToDay(this DateTime dt)
        {
            return dt.TruncateTo(TimeSpan.TicksPerDay);
        }

        public static DateTime LastInstantOfDay(this DateTime dt)
        {
            return dt.AddDays(1).TruncateToDay().AddMilliseconds(-1);
        }

        public static DateTime? LastInstantOfDay(this DateTime? dt)
        {
            return dt.HasValue
                ? dt.Value.LastInstantOfDay()
                : dt;
        }

        /// <summary>
        /// Returns the next date following start date every period numberInPeriod at hours and minutes
        /// e.g. 
        /// - 2 Days following 12.7.2016 17:20 => jeudi 14 juillet 2016 15:30:00, samedi 16 juillet 2016 15:30:00
        /// - 2 Weeks 4th day following 12.7.2016 17:20 => mercredi 27 juillet 2016 15:30:00, mercredi 10 août 2016 15:30:00
        /// - 2 Months 4th day following 12.7.2016 17:20 => dimanche 4 septembre 2016 15:30:00, vendredi 4 novembre 2016 15:30:00
        /// </summary>
        /// <param name="start">start date to compute next date</param>
        /// <param name="hours">1..24</param>
        /// <param name="minutes">1..60</param>
        /// <param name="every">1,2,3...</param>
        /// <param name="period">Day, Week or Mont</param>
        /// <param name="numberInPeriod">Week 1..7, Month 1..12</param>
        /// <returns></returns>
        public static DateTime NextDate(this DateTime start, int hours, int minutes, int every, string period, int numberInPeriod)
        {
            var dateWithHour = DateTime.Parse("0001-01-01 " + hours + ":" + minutes);
            start = new DateTime(start.Year, start.Month, start.Day);
            DateTime nextDate = start;
            if (period == "Day")
                nextDate = start.AddDays(every);
            else if (period == "Week")
                nextDate = start.FirstDayOfWeek().AddDays(every * 7 + numberInPeriod - 1);
            else if (period == "Month")
                nextDate = start.FirstDayOfMonth().AddMonths(every).AddDays(numberInPeriod - 1);
            nextDate = nextDate.AddHours(dateWithHour.Hour);
            nextDate = nextDate.AddMinutes(dateWithHour.Minute);
            return nextDate;
        }

        public static DateTime FirstDayOfWeek(this DateTime date)
        {
            return date.AddDays(-(int)date.DayOfWeek);
        }

        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static string ToString(this DateTime? dt, string format)
        {
            return dt != null
                ? ((DateTime)dt).ToString(format)
                : "";
        }

        #endregion Truncate methods

        #region Helpers

        private static DateTime TruncateTo(this DateTime dt, long numTicksToShaveOff)
        {
            return new DateTime(
                dt.Ticks - (dt.Ticks % numTicksToShaveOff),
                dt.Kind
            );
        }

        #endregion Helpers
    }
}