using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agridea
{
    /// <summary>
    /// Summary description for RandomGenerator.
    /// </summary>
    public class RandomGenerator : IRandomGenerator
    {
        private const int RandomA = 16807;
        private const int RandomM = 2147483647;
        private const int RandomQ = 127773;
        private const int RandomR = 2836;
        private const int RandomTR = 1043618065;

        private int seed_;

        ///<summary>
        /// Factory method
        ///</summary>
        public static RandomGenerator Make()
        {
            return new RandomGenerator();
        }

        ///<summary>
        /// Init the pseudo random number generator
        /// use a specific integer if determinism is required
        ///</summary>
        ///<param name="seed">The seed used to initialize the generator</param>
        public void Seed(int seed)
        {
            seed_ = seed;
        }

        ///<summary>
        /// Init the pseudo random number generator
        /// use the current time in milliseconds  
        ///</summary>
        public void SeedWithClock()
        {
            seed_ = Environment.TickCount;
        }

        ///<summary>
        /// Return a random integer value in [0..maxvalue(long)]
        ///</summary>
        public int Next()
        {
            int lo, hi, test;

            hi = seed_ / RandomQ;
            lo = seed_ % RandomQ;
            test = (RandomA * lo) - (RandomR * hi);
            seed_ = (test > 0) ? test : test + RandomM;

            return seed_;
        }

        ///<summary>
        /// Return a random integer value in [inf..sup]
        ///</summary>
        ///<param name="inf">Inferior possible value generated</param>
        ///<param name="sup">Superior possible value generated</param>
        public int Next(int inf, int sup)
        {
            return inf + Next() % (sup - inf + 1);
        }

        ///<summary>
        /// Return true with prob% probability, false with 100-prob% probability
        ///</summary>
        ///<param name="prob">Probability of true</param>
        public bool Next(int prob)
        {
            return Next(1, 100) <= prob;
        }
    }
}
