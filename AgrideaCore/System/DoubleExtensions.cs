
namespace System
{
    public static class DoubleExtensions
    {
        public const double Epsilon = 0.00001;
        public const decimal PriceEpsilon = 0.01M;

        //from http://floating-point-gui.de/errors/comparison/
        public static bool ApproximateEquals(this double left, double right)
        {
            var absA = Math.Abs(left);
            var absB = Math.Abs(right);
            var diff = Math.Abs(left - right);

            // shortcut, handles infinities
            if (left == right)
                return true;

            //a or b or both are zero, realtive error is not meaningful
            if (left * right == 0)
                return diff < (Epsilon * Epsilon);

            // use relative error
            return diff / (absA + absB) < Epsilon;
        }
        public static bool ApproximateGreater(this double left, double right)
        {
            return !left.ApproximateEquals(right) &&
                left > right;
        }
        public static bool ApproximateSmaller(this double left, double right)
        {
            return right.ApproximateGreater(left);
        }

        public static bool ApproximateGreaterOrEqual(this double left, double right)
        {
            return left.ApproximateEquals(right) || left.ApproximateGreater(right);
        }

        public static string ToEmptyDecimal(this double value, int decimals = 2)
        {
            if (value.ApproximateEquals(0)) return string.Empty;
            var format = string.Format("0.{0}", new String('0', decimals));
            return value.ToString(format);
        }

        public static double Rounded(this double value, int decimals = 2)
        {
            return Math.Round(value, decimals);
        }

        public static double Divide(this double value, double divisor = 100)
        {
            return value / divisor;
        }


        public static bool PriceEquals(this double x, double y)
        {
            return Math.Abs(x - y) < Convert.ToDouble(PriceEpsilon);
        }

        public static string ToStringWithDefault(this double value, double compareValue, string defaultValue)
        {
            if (value.PriceEquals(compareValue)) return defaultValue;
            return value.ToString();
        }
        public static double PercentIncrease(this double value, double previous)
        {
            return ((value - previous) / previous) * 100;
        }

        public static string ToHourMinuteSecondsMilliseconds(this double value)
        {
            return TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss\:fff");
        }
    }
}
