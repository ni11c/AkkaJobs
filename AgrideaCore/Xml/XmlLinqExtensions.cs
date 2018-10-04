using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Agridea.Xml.Linq
{
    public static class XmlLinqExtensions
    {
        public static string ValueOrEmpty(this IEnumerable<XElement> elements)
        {
            var single = elements.SingleOrDefault();
            return single == null ? String.Empty : single.Value;
        }
        public static int Int32ValueOrDefault(this IEnumerable<XElement> elements)
        {
            var single = elements.SingleOrDefault();
            int value = default(int);
            if (single == null)
                return value;

            int.TryParse(single.Value, out value);
            return value;
        }
        public static string ValueOrEmptyFor(this XNode node, string xpath)
        {
            if (node == null) return String.Empty;
            return node.XPathSelectElements(xpath).ValueOrEmpty();
        }
    }
}
