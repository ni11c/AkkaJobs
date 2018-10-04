using System;
using Agridea.Diagnostics.Logging;

namespace System
{
    public static class ActionExtensions
    {
        public static void Retry<T>(this Action treatement, int maxRetryCount = 1) where T : Exception
        {
            bool success = false;
            int runCount_ = maxRetryCount + 1;

            while (runCount_ > 0 && !success)
                try
                {
                    treatement();
                    success = true;
                }
                catch (T e)
                {
                    runCount_--;
                    Log.Warning(e);
                }
            if (!success) throw new ApplicationException(string.Format("Treatment failed after {0} retry", maxRetryCount));
        }

        public static TR Retry<TR,TE>(this Func<string, TR> treatment, string param, int maxRetryCount = 1) where TE : Exception
        {
            var result = default(TR);
            bool success = false;
            int runCount_ = maxRetryCount + 1;

            while (runCount_ > 0 && !success)
                try
                {
                    result = treatment(param);
                    success = true;
                }
                catch (TE e)
                {
                    runCount_--;
                    Log.Warning(e);
                }
            if (!success) throw new ApplicationException(string.Format("Treatment failed after {0} retry", maxRetryCount));
            return result;
        }
    }
}
