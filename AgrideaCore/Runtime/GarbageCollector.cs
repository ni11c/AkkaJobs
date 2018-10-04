using System;

namespace Agridea.Runtime
{
    public class GarbageCollector : IMemory
    {
        #region Constants
        private static readonly long OneByte = 1;
        private static readonly long OneKilobyte = OneByte * 1024;
        private static readonly long OneMegabyte = OneKilobyte * 1024;
        #endregion

        #region Members
        private long previousValue_ = GC.GetTotalMemory(true);
        #endregion

        #region IMemory
        public long CurrentSizeInBytes
        {
            get { return GC.GetTotalMemory(true); }
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
