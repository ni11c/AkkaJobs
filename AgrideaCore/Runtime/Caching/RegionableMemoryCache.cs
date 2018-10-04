
using Agridea.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;

namespace Agridea.Runtime.Caching
{
    /// <summary>
    /// MemoryCache with regions, all cache items with sliding expiration and lockable
    /// Thread safe for one instance
    /// </summary>
    public class RegionableMemoryCache : Disposable
    {
        #region Members
        private object SyncRoot = new object();
        private string name_;
        private long cacheMemoryLimitMegabytes_;
        private long physicalMemoryLimitPercentage_;
        private TimeSpan pollingInterval_;
        private EnumerableMemoryCache memoryCache_;
        private CacheItemPolicy cacheItemPolicy_;
        #endregion

        #region Initialization
        public RegionableMemoryCache(string name, long cacheMemoryLimitMegabytes, long physicalMemoryLimitPercentage, TimeSpan pollingInterval, TimeSpan slidingExpiration)
        {
            name_ = name;
            cacheMemoryLimitMegabytes_ = cacheMemoryLimitMegabytes;
            physicalMemoryLimitPercentage_ = physicalMemoryLimitPercentage;
            pollingInterval_ = pollingInterval;

            cacheItemPolicy_ = new CacheItemPolicy { SlidingExpiration = slidingExpiration };
            cacheItemPolicy_.RemovedCallback = RemovedCallback;

            CreateCache(name_, cacheMemoryLimitMegabytes_, physicalMemoryLimitPercentage_, pollingInterval_);
        }
        #endregion

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            Log.Info(string.Format("'{0}:{1}' Dispose()", GetType().FullName, GetHashCode()));
            if (disposing)
            {
                cacheItemPolicy_.RemovedCallback = null;

                memoryCache_.Dispose();
                memoryCache_ = null;
            }
        }
        #endregion

        #region Services
        #region Global
        public void Clear()
        {
            lock (SyncRoot)
            {
                memoryCache_.Dispose();
                memoryCache_ = null;

                CreateCache(name_, cacheMemoryLimitMegabytes_, physicalMemoryLimitPercentage_, pollingInterval_);
            }
        }
        #endregion

        #region Individual Items
        public bool Contains(string key, string regionName = null)
        {
            lock (SyncRoot)
            {
                return Get(key, false, regionName) != null;
            }
        }
        public int Count()
        {
            lock (SyncRoot)
            {
                return memoryCache_.GetCountWithPrefix(null);
            }
        }
        public object Get(string key, bool lockIt = false, string regionName = null)
        {
            lock (SyncRoot)
            {
                var value = memoryCache_.Get(EnumerableMemoryCache.BuildKey(key, regionName)) as LockableValue;
                if (value == null) return null;
                if (value.Locked) throw new ObjectLockedException(string.Format("Item with key '{0}' is locked", key));
                if (lockIt) Add(key, value.Value, unLockit: false, regionName: regionName);
                return value.Value;
            }
        }
        public void Add(string key, object value, bool unLockit = true, string regionName = null)
        {
            lock (SyncRoot)
            {
                var item = memoryCache_.Get(EnumerableMemoryCache.BuildKey(key, regionName)) as LockableValue;
                if (item == null)
                {
                    memoryCache_.Add(new CacheItem(EnumerableMemoryCache.BuildKey(key, regionName), new LockableValue(value, locked: false)), cacheItemPolicy_);
                    return;
                }
                item.Value = value;
                item.Locked = !unLockit && item.Locked;
            }
        }
        public void Remove(string key, string regionName = null)
        {
            lock (SyncRoot)
            {
                if (!Contains(EnumerableMemoryCache.BuildKey(key, regionName))) return;
                memoryCache_.Remove(EnumerableMemoryCache.BuildKey(key, regionName));
            }
        }
        #endregion

        #region Regions
        public bool ContainsRegion(string regionName)
        {
            lock (SyncRoot)
            {
                return memoryCache_.ContainsKeyWithPrefix(regionName);
            }
        }
        public int CountRegion(string regionName, bool recursively = false)
        {
            lock (SyncRoot)
            {
                return memoryCache_.GetCountWithPrefix(regionName, recursively);
            }
        }
        public IEnumerable<KeyValuePair<string, object>> GetRegion(string regionName, bool recursively = false, bool lockIt = false)
        {
            lock (SyncRoot)
            {
                var internalRegion = memoryCache_.GetAll(regionName, recursively).ToList();
                if (!internalRegion.Any()) return internalRegion;
                if (internalRegion.Any(x => (x.Value as LockableValue).Locked)) throw new ObjectLockedException(string.Format("Region with name '{0}' is locked", regionName));
                var region = internalRegion.Select(x => new KeyValuePair<string, object>(x.Key, (x.Value as LockableValue).Value));
                if (lockIt) AddRegion(regionName, region, !lockIt);
                return region;
            }
        }
        public void AddRegion(string regionName, IEnumerable<KeyValuePair<string, object>> region, bool unlockIt = true)
        {
            lock (SyncRoot)
            {
                RemoveRegion(regionName);

                foreach (var item in region)
                    memoryCache_.Add(new CacheItem(EnumerableMemoryCache.BuildKey(item.Key, null), new LockableValue(item.Value, locked: false)), cacheItemPolicy_);
            }
        }
        public void RemoveRegion(string regionName, bool recursively = false)
        {
            lock (SyncRoot)
            {
                foreach (var item in GetRegion(regionName: regionName, recursively: recursively)) memoryCache_.Remove(item.Key);
            }
        }
        #endregion
        #endregion

        #region Helpers
        private void CreateCache(string name, long cacheMemoryLimitMegabytes, long physicalMemoryLimitPercentage, TimeSpan pollingInterval)
        {
            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("cacheMemoryLimitMegabytes", cacheMemoryLimitMegabytes.ToString());
            nameValueCollection.Add("physicalMemoryLimitPercentage", physicalMemoryLimitPercentage.ToString());
            nameValueCollection.Add("pollingInterval", pollingInterval.ToString());
            memoryCache_ = new EnumerableMemoryCache(name, nameValueCollection);
        }
        private void RemovedCallback(CacheEntryRemovedArguments arguments)
        {
            Log.Info("RemovedCallback('{0}')", arguments.CacheItem.Key);
        }
        #endregion
    }

}
