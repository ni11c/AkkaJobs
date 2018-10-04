using System;
using System.Diagnostics;
using System.Reflection;

namespace Agridea.Core
{
    /// In Debug : Check, logs (including derivatives of WarningException) and throws.
    /// In Release : does nothing, code not even compiled
    /// See guidelines in Requires
    public static class Asserts<T> where T : Exception, new()
    {
        #region Services
        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string message = null, Exception innerException = null)
        {
            if (condition) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void IsFalse(bool condition, string message = null, Exception innerException = null)
        {
            if (!condition) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void Or(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a || b) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void And(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a && b) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void Xor(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a.Xor(b)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void Nand(bool a, bool b, string message = null, Exception innerException = null)
        {
            if (a.Nand(b)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void Implies(bool hypothesis, bool conclusion, string message = null, Exception innerException = null)
        {
            if (hypothesis.Implies(conclusion)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void IsNotNull(object instance, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(instance, null)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void IsNull(object instance, string message = null, Exception innerException = null)
        {
            if (object.ReferenceEquals(instance, null)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void AreSame(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (object.ReferenceEquals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void AreNotSame(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void AreEqual(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (object.Equals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void AreNotEqual(object expected, object actual, string message = null, Exception innerException = null)
        {
            if (!object.Equals(expected, actual)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void IsNotEmpty(string instance, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(instance, null) && instance.Length > 0) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void IsEmpty(string instance, string message = null, Exception innerException = null)
        {
            if (!object.ReferenceEquals(instance, null) && instance.Length == 0) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void IsInstanceOf(object instance, Type type, string message = null, Exception innerException = null)
        {
            if (type.IsInstanceOfType(instance)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void LessThan(int val, int max, string message = null, Exception innerException = null)
        {
            if (val < max) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void LessOrEqual(int val, int max, string message = null, Exception innerException = null)
        {
            if (val <= max) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void GreaterThan(int val, int min, string message = null, Exception innerException = null)
        {
            if (val > min) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void GreaterOrEqual(int val, int min, string message = null, Exception innerException = null)
        {
            if (val >= min) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void InStrictRange(int val, int min, int max, string message = null, Exception innerException = null)
        {
            if (val > min && val < max) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void InRange(int val, int min, int max, string message = null, Exception innerException = null)
        {
            if (val >= min && val <= max) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void Contains(string superstring, string substring, string message = null, Exception innerException = null)
        {
            if (superstring.Contains(substring)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
        public static void DoesNotContain(string superstring, string substring, string message = null, Exception innerException = null)
        {
            if (!superstring.Contains(substring)) return;
            Throw<T>(message, innerException);
        }
        [Conditional("DEBUG")]
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
            else
                exception = typeof(E).InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { message, innerException }) as E;

            Log.Error(string.Format("Assert violation : {0}", exception.Message));
            throw (exception as E);
        }
        #endregion
    }
}
