using System.Linq;

namespace System
{
    public static class IntegerExtensions
    {
        public static bool In(this Int32 i, params Int32[] list)
        {
            return list.Contains(i);
        }
        public static bool Between(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
        public static int Limit(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
        public static string ToStringWithDefault(this int value, string defaultValue)
        {
            if (value == default(int)) return defaultValue;
            return value.ToString();
        }
        public static double PercentIncrease(this int value, int previous)
        {
            return ((Convert.ToDouble(value) - Convert.ToDouble(previous)) / Convert.ToDouble(previous)) * 100;
        }
    }
}
