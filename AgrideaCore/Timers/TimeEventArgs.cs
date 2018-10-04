using System;

namespace Agridea.Timers
{
    [Serializable]
    public class TimeEventArgs : EventArgs
    {
        #region Properties
        public long Time { get; private set; }
        #endregion

        #region Initialization
        public TimeEventArgs(long time)
        {
            Time = time;
        }
        public override string ToString()
        {
            return base.ToString() + ":" + Time.ToString();
        }
        #endregion
    }
}
