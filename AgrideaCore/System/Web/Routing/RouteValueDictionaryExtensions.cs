using System.Collections.Generic;
using System.Linq;
using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc;

namespace System.Web.Routing
{
    public static class RouteValueDictionaryExtensions
    {
        #region Services
        /// <summary>
        /// Adds a range of route value
        /// </summary>
        /// <param name="routeValuesToAdd">route values to be added</param>
        /// <returns></returns>
        public static RouteValueDictionary AddRange(this RouteValueDictionary routeValues, RouteValueDictionary routeValuesToAdd)
        {
            if (null == routeValuesToAdd)
            {
                return routeValues;
            }

            foreach (var entry in routeValuesToAdd)
            {
                routeValues.Add(entry.Key, entry.Value);
            }

            return routeValues;
        }

        /// <summary>
        /// Adds a range of route value
        /// </summary>
        /// <param name="routeValuesToAdd">a dictionary of route values to be added</param>
        /// <returns></returns>
        public static RouteValueDictionary AddRange(this RouteValueDictionary routeValues, IDictionary<string, object> routeValuesToAdd)
        {
            if (null == routeValuesToAdd)
            {
                return routeValues;
            }
            return routeValues.AddRange(new RouteValueDictionary(routeValuesToAdd));
        }

        /// <summary>
        /// Checks whether a dictionary of route values contain all the route values of another dictionary
        /// </summary>
        /// <param name="container">the dictionary on which the check is made.</param>
        /// <param name="contained">the values to be contained</param>
        /// <remarks>
        /// The default equality comparer that is used is a basic ToString() comparer, ok for string and numeric types but may be irrelevant for
        /// other types. Use at your own risks.
        /// </remarks>
        /// <returns>true if container contains contained, false otherwise</returns>
        public static bool Contains(this RouteValueDictionary container, RouteValueDictionary contained)
        {
            return Contains(container, contained, (x, y) => x.ToString() == y.ToString());
        }

        /// <summary>
        /// Checks whether a dictionary of route values contain all the route values of another dictionary
        /// </summary>
        /// <param name="container">the dictionary on which the check is made.</param>
        /// <param name="contained">the values to be contained</param>
        /// <param name="comparer">The equality comparer used to compare route values for equality</param>
        /// <returns>true if container contains contained, false otherwise</returns>
        public static bool Contains(this RouteValueDictionary container, RouteValueDictionary contained, Func<object, object, bool> equalityComparer)
        {
            Requires<ArgumentNullException>.IsNotNull(container);
            Requires<ArgumentNullException>.IsNotNull(contained);

            object foo;
            foreach (string key in contained.Keys)
            {
                foo = null;
                if (!container.TryGetValue(key, out foo))
                {
                    return false;
                }
                if (!equalityComparer(contained[key], foo))
                {
                    return false;
                }
            }
            return true;
        }
        public static IList<KeyValuePair<TKey, TValue>> ToKeyValueItem<T, TKey, TValue>(this IList<T> list, Func<T, TKey> key, Func<T, TValue> value)
        {
            return (from item in list
                    select new KeyValuePair<TKey, TValue>(key(item), value(item))
                ).ToList();
        }
        /// <summary>
        /// Removes all keys but controller and action
        /// </summary>
        public static RouteValueDictionary PurgeQueryString(this RouteValueDictionary routeValueDict)
        {
            var copy = new RouteValueDictionary(routeValueDict);
            foreach (var item in routeValueDict)
            {
                if (item.Key != MvcConstants.ControllerRouteValueKey && item.Key != MvcConstants.ActionRouteValueKey)
                    copy.Remove(item.Key);
            }
            return copy;
        }
        #endregion
    }
}
