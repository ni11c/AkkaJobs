using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Agridea.Diagnostics.Logging
{
    public class ProgressIndicator
    {
        private int max_;
        private int counter_;
        public static event EventHandler<ProgressIndicatorEventArgs> OnePercent;
        public static event EventHandler<ProgressIndicatorEventArgs> TenPercent;
        public static event EventHandler<ProgressIndicatorEventArgs> FiftyPercent;

        public double Percent
        {
            get { return counter_ * 100.0 / max_; }
        }

        public ProgressIndicator(int max)
        {
            max_ = max;
            counter_ = 0;
        }

        public ProgressIndicator Reset()
        {
            counter_ = 0;
            return this;
        }

        public ProgressIndicator Tick()
        {
            double previousPercent = Percent;
            counter_++;
            if (counter_ == max_) Reset();
            if (Convert.ToInt32(Math.Floor(previousPercent)) != Convert.ToInt32(Math.Floor(Percent)))
                RaiseOnePercent();
            if (Convert.ToInt32(Math.Floor(previousPercent / 10)) != Convert.ToInt32(Math.Floor(Percent / 10)))
                RaiseTenPercent();
            if (previousPercent < 50.0 && Percent >= 50.0)
                RaiseFiftyPercent();
            return this;
        }

        protected void RaiseOnePercent()
        {
            try
            {
                OnePercent(null, new ProgressIndicatorEventArgs { Percent = this.Percent });
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
        protected void RaiseTenPercent()
        {
            try
            {
                TenPercent(null, new ProgressIndicatorEventArgs { Percent = this.Percent });
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
        protected void RaiseFiftyPercent()
        {
            try
            {
                FiftyPercent(null, new ProgressIndicatorEventArgs { Percent = this.Percent });
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
    }

    public class ProgressIndicatorEventArgs : EventArgs
    {
        public double Percent { get; set; }
    }
}
