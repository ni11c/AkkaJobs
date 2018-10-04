using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class DecimalExtensions
    {
        public static decimal RoundToFiveCents(this decimal value)
        {
            return Math.Floor(value*20)/20;
        }

        //this method comes directly from BBS project, should be used when rounding for "Acorda" BBS variables
        public static Decimal RoundToSwissCentime(this Decimal input)
        {
            return Math.Round(input * new Decimal(200, 0, 0, false, 1), MidpointRounding.AwayFromZero) * new Decimal(5, 0, 0, false, 2);
        }

    }
}
