using System;
using System.Reflection;
using Agridea.Diagnostics.Logging;
using Agridea.Runtime;

namespace Agridea.Diagnostics.Contracts
{
    /// <summary>
    /// In Debug and Release : Check, logs (except for derivative of WarningException) and throws.
    /// Guidelines : 
    /// - use Requires for checking data coming from outside and/or for achieving control (beware or perf. impact)
    /// - use Asserts for checking internal assumptions of code during development
    /// 
    /// Add at will :
    /// - RegexMatch
    /// ...
    /// </summary>
    public static class Requires<T> where T : Exception, new()
    {
        #region Services
        public static void IsTrue(bool condition, string message = null, Exception innerException = null)
        {
            if (condition) return;
            Throw<T>(message, innerException);
        }
        public static void IsFalse(bool condition, string message = null, Exception innerException = null)
        {
            if (!condition) return;
            Throw<T>(message, innerException);
        }
        public static void Or(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a || b) return;
            Throw<T>(message, innerException);
        }
        public static void And(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a && b) return;
            Throw<T>(message, innerException);
        }
        public static void Xor(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a.Xor(b)) return;
            Throw<T>(message, innerException);
        }
        public static void Nand(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a.Nand(b)) return;
            Throw<T>(message, innerException);
        }
        public static void Implies(bool hypothesis, bool conclusion, string message = null, Exception innerException = null)
        {
            if (hypothesis.Implies(conclusion)) return;
            Throw<T>(message, innerException);
        }
        public static void IsNotNull(object instance, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(instance, null)) return;
            Throw<T>(message, innerException);
        }
        public static void IsNull(object instance, string message = null, Exception innerException = null)
        {
            if (object.ReferenceEquals(instance, null)) return;
            Throw<T>(message, innerException);
        }
        public static void AreSame(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (object.ReferenceEquals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        public static void AreNotSame(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        public static void AreEqual(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (object.Equals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        public static void AreNotEqual(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (!object.Equals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        public static void IsNotEmpty(string instance, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(instance, null) && instance.Length > 0) return;
            Throw<T>(message, innerException);
        }
        public static void IsEmpty(string instance, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(instance, null) && instance.Length == 0) return;
            Throw<T>(message, innerException);
        }
        public static void IsInstanceOf(object instance, Type type, string message = null, Exception innerException = null)
        {
            if (type.IsInstanceOfType(instance)) return;
            Throw<T>(message, innerException);
        }
        public static void LessThan(int val, int max, string message = null, Exception innerException = null)
        {
            if (val < max) return;
            Throw<T>(message, innerException);
        }
        public static void LessOrEqual(int val, int max, string message = null, Exception innerException = null)
        {
            if (val <= max) return;
            Throw<T>(message, innerException);
        }
        public static void GreaterThan(int val, int min, string message = null, Exception innerException = null)
        {
            if (val > min) return;
            Throw<T>(message, innerException);
        }
        public static void GreaterOrEqual(int val, int min, string message = null, Exception innerException = null)
        {
            if (val >= min) return;
            Throw<T>(message, innerException);
        }
        public static void InStrictRange(int val, int min, int max, string message = null, Exception innerException = null)
        {
            if (val > min && val < max) return;
            Throw<T>(message, innerException);
        }
        public static void InRange(int val, int min, int max, string message = null, Exception innerException = null)
        {
            if (val >= min && val <= max) return;
            Throw<T>(message, innerException);
        }
        public static void Contains(string superstring, string substring, string message = null, Exception innerException = null)
        {
            if (superstring.Contains(substring)) return;
            Throw<T>(message, innerException);
        }
        public static void DoesNotContain(string superstring, string substring, string message = null, Exception innerException = null)
        {
            if (!superstring.Contains(substring)) return;
            Throw<T>(message, innerException);
        }
        public static void Fails(string message = null, Exception innerException = null)
        {
            Throw<T>(message, innerException);
        }
        #endregion

        #region Helpers
        private static void Throw<E>(string message = null, Exception innerException = null) where E : Exception, new()
        {
            E exception = null;

            if (message == null && innerException == null)
                exception = typeof(E).InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { }) as E;
            else if (innerException == null)
                exception = typeof(E).InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { message }) as E;
            //NOTE the case message == null && innerException != null is prevented from the compiler (missing args must be the last ones)
            else
                exception = typeof(E).InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { message, innerException }) as E;

            if(! typeof(E).IsSubtypeOf(typeof(WarningException))) Log.Error(string.Format("Requires violation : {0}", exception.Message));
            throw (exception as E);
        }
        #endregion
    }
}
