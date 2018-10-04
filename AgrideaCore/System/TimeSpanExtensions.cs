using System;

namespace System
{
    public static class TimeExtensions
    {
        #region Services
        public static string DisplayAsHourMinuteSeconds(this TimeSpan timeSpan)
        {
            return string.Format("{0}:{1}:{2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
        #endregion
    }
}
