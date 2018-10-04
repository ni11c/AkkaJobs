using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;
using Agridea.Diagnostics.Contracts;

namespace Agridea.Web.Mvc
{
    public static class RouteValueDictionaryExtensions
    {
        public const string Class = "class";
        
        public static RouteValueDictionary Merge(this RouteValueDictionary original, RouteValueDictionary @new, bool replace = true)
        {
            if (@new == null)
                return original;

            original = original.MergeAndCreate(@new, replace);
            return original;
        }

        public static RouteValueDictionary MergeAndCreate(this RouteValueDictionary first, RouteValueDictionary second, bool replace = true)
        {
            Requires<ArgumentNullException>.IsNotNull(first);

            RouteValueDictionary merged = new RouteValueDictionary(first);
            if (second == null)
                return merged;
            foreach (var item in second)
            {
                if (merged.Keys.Contains(item.Key))
                    merged[item.Key] = replace ? item.Value : merged[item.Key];
                else
                    merged.Add(item.Key, item.Value);
            }
            return merged;
        }

        public static RouteValueDictionary AddClass(this RouteValueDictionary original, string className)
        {
            RouteValueDictionary output = new RouteValueDictionary(original);
            if (string.IsNullOrWhiteSpace(className)) return output;

            if (!output.ContainsKey(Class)) output.Add(Class, className);
            else
            {
                string classList = (string)output[Class];
                string pattern = string.Format("\\b{0}\\b", className);
                if (!Regex.IsMatch(classList, pattern))
                    classList = string.Join(" ", classList, className);
                output[Class] = classList;
            }

            return output;
        }
    }
}
