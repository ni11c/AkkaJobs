namespace System
{
    using Linq;

    public static class BooleanExtensions
    {
        #region Services

        public static bool Implies(this bool hypothesis, bool conclusion)
        {
            return !hypothesis || conclusion;
        }

        public static bool IsEquivalentTo(this bool hypothesis, bool conclusion)
        {
            return hypothesis == conclusion;
        }

        public static bool Xor(this bool firstTerm, bool secondTerm)
        {
            if (firstTerm && secondTerm) return false;
            if (!firstTerm && !secondTerm) return false;
            return true;
        }

        public static bool Xnor(this bool firstTerm, bool secondTerm)
        {
            return !Xor(firstTerm, secondTerm);
        }

        public static bool Nand(this bool firstTerm, bool secondTerm)
        {
            if (firstTerm && secondTerm) return false;
            return true;
        }

        public static string ToYesNo(this bool source)
        {
            return source ? "Oui" : "Non";
        }

        public static string ToYesBlank(this bool source)
        {
            return source ? "Oui" : "";
        }

        public static int ToZeroOne(this bool source)
        {
            return source
                ? 1
                : 0;
        }

        public static bool OnlyOneIsTrue(params bool[] terms)
        {
            return terms.Select(Convert.ToInt32).Sum() == 1;
        }

        public static int AsInt(this bool b)
        {
            return b ? 1 : 0;
        }
        #endregion Services
    }
}