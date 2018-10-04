using System.Collections.Generic;

namespace System
{
    public static class NullSafeExtensions
    {
        public static TResult NullSafe<TCheck, TResult>(this TCheck check, Func<TCheck, TResult> valueIfNotNull, TResult defaultValue = default(TResult))
            where TCheck : class
        {
            return check == null ? defaultValue : valueIfNotNull(check);
        }

        public static string ToDisplayString<T>(this T? value, string defaultValue = "@null")
             where T : struct
        {
            if (!value.HasValue) return defaultValue;
            return value.Value.ToString();
        }

        public static string ToDisplayString<T>(this IList<T> value, string defaultValue = "@null")
        {
            if (value == null) return defaultValue;
            return string.Format("@count:{0}", value.Count);
        }
    }
}