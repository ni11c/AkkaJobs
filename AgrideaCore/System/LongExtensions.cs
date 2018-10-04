
namespace System
{
    public static class LongExtensions
    {
        #region Constants
        //remarks see http://www.t1shopper.com/tools/calculate/
        //this fits how Windows display file size in explorer
        private static readonly long Kilo = 1000;
        private static readonly long KiloByte = 1024;
        private static readonly long MegaByte = Kilo * KiloByte;
        private static readonly long GigaByte = Kilo * MegaByte;
        #endregion

        #region Services
        public static string DisplayAsFileSizeInGbMbAndKb(this long sizeInBytes)
        {
            long bytes = 0;
            long kiloBytes = 0;
            long megaBytes = 0;
            long gigaBytes = Math.DivRem(sizeInBytes, GigaByte, out megaBytes);
            megaBytes = Math.DivRem(megaBytes, MegaByte, out kiloBytes);
            kiloBytes = Math.DivRem(kiloBytes, KiloByte, out bytes);

            return string.Format("{0}'{1}'{2}Kb", gigaBytes, megaBytes, kiloBytes);
        }
        public static string ToThousandsSeparated(this long value)
        {
            return string.Format("{0:#,##0}", value);
        }
        public static double PercentIncrease(this long value, long previous)
        {
            return ((Convert.ToDouble(value) - Convert.ToDouble(previous)) / Convert.ToDouble(previous)) * 100;
        }
        #endregion
    }
}
