using System.Collections.Generic;
using System.Linq;

namespace System.Xml.Linq
{
    public static class XElementExtensions
    {
        public static string ValueOrDefault(this IEnumerable<XElement> elements)
        {
            var temp = elements.FirstOrDefault();
            return temp == null ? "" : temp.Value;
        }

        public static T ValueOrDefault<T>(this IEnumerable<XElement> elements)
        {
            var temp = elements.FirstOrDefault();
            return temp == null ? default(T) : (T) Convert.ChangeType(temp.Value, typeof(T));
        }

        public static T? ValueOrNull<T>(this IEnumerable<XElement> elements)
            where T : struct
        {
            var temp = elements.FirstOrDefault();
            return temp != null ? (T?) Convert.ChangeType(temp.Value, typeof(T)) : null;
        }
    }
}