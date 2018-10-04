using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Agridea.Diagnostics.Logging;

namespace System
{
    public static class CollectionExtensions
    {
        public static IList<TItem> Clone<TItem>(this IList<TItem> list) where TItem : class, ICloneable
        {
            var clone = new List<TItem>();
            list.ToList().ForEach(x => clone.Add(x.Clone() as TItem));
            return clone;
        }
        public static IEnumerable<T> Add<T>(this IEnumerable<T> enumerable, T item)
        {
            return (enumerable ?? Enumerable.Empty<T>()).Concat(new[] { item });
        }
        public static T[] AddRangeToArray<T>(this T[] sequence, T[] items)
        {
            return (sequence ?? Enumerable.Empty<T>()).Concat(items).ToArray();
        }
        public static T[] AddToArray<T>(this T[] array, T item)
        {
            return Add(array, item).ToArray();
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            //HashSet use an Equality comparer to walk a list sequentially...
            //Dictionary is more efficient
            Dictionary<TKey, TSource> dict = new Dictionary<TKey, TSource>();

            foreach (TSource element in source)
            {
                var key = keySelector(element);

                if (dict.ContainsKey(key)) continue;

                dict.Add(key, element);
                yield return element;
            }
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var d = new Dictionary<string, string>();
            if (collection == null)
                return d;

            foreach (var key in collection.AllKeys.Where(key => !d.ContainsKey(key))) {
                d.Add(key, collection[key]);
            }
            return d;
        }
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;

            var newValue = valueFactory(key);
            dictionary.Add(key, newValue);
            return newValue;
        }
    }
}
