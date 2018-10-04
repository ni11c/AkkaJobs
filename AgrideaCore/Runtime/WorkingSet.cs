using System;

namespace Agridea.Runtime
{
    public class WorkingSet : IMemory
    {
        #region Constants
        public static readonly long OneByte = 1;
        public static readonly long OneKilobyte = OneByte * 1024;
        public static readonly long OneMegabyte = OneKilobyte * 1024;
        #endregion

        #region Members
        private long previousValue_ = Environment.WorkingSet;
        #endregion

        #region IMemory
        public long CurrentSizeInBytes
        {
            get { return Environment.WorkingSet; }
            set { }
        }
        public double CurrentSizeInKiloBytes
        {
            get { return Convert.ToDouble(CurrentSizeInBytes) / OneKilobyte; }
            set { }
        }
        public double CurrentSizeInMegaBytes
        {
            get { return Convert.ToDouble(CurrentSizeInBytes) / OneMegabyte; }
            set { }
        }
        public long UsedBytes
        {
            get
            {
                var currentSizeInBytes = CurrentSizeInBytes;
                var value = Math.Max(0, currentSizeInBytes - previousValue_);
                previousValue_ = currentSizeInBytes;
                return value;
            }
        }
        #endregion
    }
}
