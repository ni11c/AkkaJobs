using System;
using System.Diagnostics;
using System.Threading;

namespace Agridea.Timers
{
    public class Time : ITime
    {
        #region Constants
        private const int TickCountPerMilliSecond = 1;
        private const int TickCountPerSecond = TickCountPerMilliSecond * 1000;
        #endregion

        #region Members
        private Stopwatch watch_;
        private long previousValue_;
        #endregion

        #region Initialization

        public Time()
        {
            watch_ = Stopwatch.StartNew();
            previousValue_ = watch_.ElapsedMilliseconds;
        }
        #endregion

        #region ITime
        public long CurrentTime
        {
            get { return watch_.ElapsedTicks; }
            set { }
        }
        public long TicksPerSecond
        {
            get { return Stopwatch.Frequency; }
        }
        public double ConvertToSeconds(long ticks)
        {
            return Convert.ToDouble(ticks) / TickCountPerSecond;
        }
        public void Sleep(int delayInMilliseconds)
        {
            Thread.Sleep(delayInMilliseconds);
        }
        public long CurrentTimeMilliseconds
        {
            get { return watch_.ElapsedMilliseconds; }
            set { }
        }
        public long ElapsedMilliseconds
        {
            get
            {
                var currentTimeMilliseconds = CurrentTimeMilliseconds;
                var value = Math.Max(0, currentTimeMilliseconds - previousValue_);
                previousValue_ = currentTimeMilliseconds;
                return value;
            }
        }
        #endregion
    }
}
