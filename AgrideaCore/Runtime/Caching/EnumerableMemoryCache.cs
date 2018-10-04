
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;

namespace Agridea.Runtime.Caching
{
    public class EnumerableMemoryCache : MemoryCache, IEnumerable<KeyValuePair<string, object>>
    {
        #region Constants
        private const char KeySeparator = '.';
        #endregion

        #region Initialization
        public EnumerableMemoryCache() : this("default", null) { }
        public EnumerableMemoryCache(string name, NameValueCollection config = null) : base(name, config) { }
        #endregion

        #region IEnumerable<KeyValuePair<string, object>>
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return base.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }
        #endregion

        #region Services
        #region Static
        public static string BuildKey(string key, string keyPrefix)
        {
            if (string.IsNullOrWhiteSpace(keyPrefix) || key.StartsWith(keyPrefix)) return key;
            return string.Format("{0}{1}{2}", keyPrefix, KeySeparator, key);
        }
        public static string BuildKey(string subKey, string key, string keyPrefix)
        {
            return BuildKey(subKey, BuildKey(key, keyPrefix));
        }
        public static string UnbuildKey(string key, string keyPrefix)
        {
            if (string.IsNullOrWhiteSpace(keyPrefix) || !key.StartsWith(keyPrefix)) return key;
            return key.Substring(key.IndexOf(keyPrefix) + keyPrefix.Length + 1);
        }
        #endregion

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return this;
        }
        public IEnumerable<KeyValuePair<string, object>> GetAll(string keyPrefix, bool recursively = false)
        {
            var all = GetAll();
            if (string.IsNullOrWhiteSpace(keyPrefix)) return all;
            if (recursively) return all.Where(kvp => IsAnyLevel(kvp.Key, keyPrefix));
            return all.Where(kvp => IsFirstLevel(kvp.Key, keyPrefix));
        }
        public bool ContainsKeyWithPrefix(string keyPrefix)
        {
            return GetAll(keyPrefix).Any();
        }
        public int GetCountWithPrefix(string keyPrefix, bool recursively = false)
        {
            if (string.IsNullOrWhiteSpace(keyPrefix)) return GetAll().Count();
            return GetAll(keyPrefix, recursively).Count();
        }
        #endregion

        #region Helpers
        private bool IsAnyLevel(string key, string keyPrefix)
        {
            return key.StartsWith(keyPrefix);
        }
        private bool IsFirstLevel(string key, string keyPrefix)
        {
            if (!key.StartsWith(keyPrefix)) return false;
            return UnbuildKey(key, keyPrefix).IndexOf(KeySeparator) == -1;
        }
        #endregion
    }
}
