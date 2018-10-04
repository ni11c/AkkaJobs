using System;
using System.Timers;

namespace Agridea.Timers
{
    public class Timer : ITimer
    {
        #region Initialization
        private System.Timers.Timer timer_;
        private Time time_;

        public Timer()
        {
            timer_ = new System.Timers.Timer();
            timer_.Elapsed += OnElapsed;
            time_ = new Time();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            timer_.Dispose();
        }
        #endregion

        #region ITimer
        public int Interval
        {
            get { return Convert.ToInt32(timer_.Interval); }
        }

        public void Start(int intervalInMilliseconds)
        {
            timer_.Interval = Convert.ToDouble(intervalInMilliseconds);
            timer_.Start();
        }

        public void Stop()
        {
            timer_.Stop();
        }

        public event TimeEventHandler Tick;
        #endregion

        #region EventHanding
        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (Tick != null) Tick(this, new TimeEventArgs(time_.CurrentTime));
        }
        #endregion
    }
}
