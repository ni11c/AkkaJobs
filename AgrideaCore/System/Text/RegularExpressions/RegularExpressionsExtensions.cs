using System;
using System.Collections.Generic;

namespace System.Text.RegularExpressions
{
    public static class RegularExpressionsExtensions
    {
        public static Group FirstOrDefault(this GroupCollection collection, Func<Group, bool> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("The group collection is null.");
            if (predicate == null)
                throw new ArgumentNullException("The predicate is null.");
            if (collection.Count == 0)
                throw new ArgumentNullException("the group collection is empty.");

            for (int i = 0; i < collection.Count; i++)
                if (predicate(collection[i]) && i != 0) return collection[i];
            return null;
        }

        public static Group LastOrDefault(this GroupCollection collection, Func<Group, bool> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("The group collection is null.");
            if (predicate == null)
                throw new ArgumentNullException("The predicate is null.");
            if (collection.Count == 0)
                throw new ArgumentNullException("the group collection is empty.");

            var candidates = new Stack<Group>();
            for (int i = 0; i < collection.Count; i++)
                if (predicate(collection[i]) && i != 0) candidates.Push(collection[i]);
            return candidates.Count > 0 ? candidates.Pop() : null;
        }

        public static IList<Group> Pick(this GroupCollection collection, Func<Group, bool> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("The group collection is null.");
            if (predicate == null)
                throw new ArgumentNullException("The predicate is null.");
            if (collection.Count == 0)
                throw new ArgumentNullException("the group collection is empty.");

            var list = new List<Group>();
            for (int i = 0; i < collection.Count; i++)
                if (predicate(collection[i]) && i != 0) list.Add(collection[i]);
            return list;
        }
    }
}
