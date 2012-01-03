using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;
using NHibernate.Cache;

namespace NHibernate.Caches.AppFabric.Adapters
{
    public class AppFabricCacheRegionAdapter : AppFabricCacheAdapter
    {
        #region Constants

        private const string DefaultCacheName = "nhibernate";

        #endregion

        #region Member variables

        private readonly string _regionName;
        private readonly DataCache _cache;

        // TODO: This needs to be moved into the distributed cache
        private IDictionary<string, DataCacheLockHandle> _locks = new Dictionary<string, DataCacheLockHandle>();

        #endregion

        #region Constructor

        public AppFabricCacheRegionAdapter(string regionName, IDictionary<string, string> properties)
            : this(regionName, new AppFabricCacheFactory(properties), properties)
        { }

        public AppFabricCacheRegionAdapter(string regionName,
                                           IAppFabricCacheFactory cacheFactory,
                                           IDictionary<string, string> properties)
            : base(regionName, properties)
        {
            // TODO: Need to be able to recreate this if the cache cluster is restarted - think this will require specific
            // code in each method
            _regionName = regionName.GetHashCode().ToString();

            // TODO: Get the cache name from the config if it exists
            _cache = cacheFactory.GetCache(DefaultCacheName, true);

            try
            {
                _cache.CreateRegion(_regionName);
            }
            catch (DataCacheException)
            {
                // TODO: Log it and swallow it if it is because it already exists?
                // TODO: A temporary failure keeps occurring here :O(
                throw;
            }
        }

        #endregion

        #region Methods

        public override void Clear()
        {
            try
            {
                _cache.ClearRegion(_regionName);
            }
            catch (DataCacheException ex)
            {
                // TODO: Do we need to check error numbers?
                throw new CacheException(ex);
            }
        }

        public override void Destroy()
        {
            try
            {
                _cache.RemoveRegion(_regionName);
            }
            catch (DataCacheException ex)
            {
                // TODO: Do we need to do anything more than just swallow?
                throw new CacheException(ex);
            }
        }

        public override object Get(object key)
        {
            if (key == null)
                return null;

            // TODO: We at least need to recover if the region has gone missing
            try
            {
                return _cache.Get(key.ToString(), _regionName);
            }
            catch (DataCacheException ex)
            {
                if (IsSafeToIgnore(ex))
                    return null;
                else
                {
                    throw new CacheException(ex);
                }
            }
        }

        public override void Lock(object key)
        {
            DataCacheLockHandle lockHandle = null;

            try
            {
                _cache.GetAndLock(key.ToString(), TimeSpan.FromMilliseconds(Timeout), out lockHandle, _regionName);
                _locks.Add(key.ToString(), lockHandle);
            }
            catch (DataCacheException ex)
            {
                // TODO: ??? WHat should go here??? Do we need to check error codes?
                throw new CacheException(ex);
            }
        }

        public override void Put(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key", "null key not allowed");

            if (value == null)
                throw new ArgumentNullException("value", "null value not allowed");

            DataCacheLockHandle lockHandle = null;

            try
            {
                _cache.GetAndLock(key.ToString(), TimeSpan.FromMilliseconds(Timeout), out lockHandle, _regionName, true);
                _cache.Put(key.ToString(), value, _regionName);
            }
            catch (DataCacheException ex)
            {
                // TODO: Need top check error codes for things like region not existing. Should it throw an exception though?
                if (!IsSafeToIgnore(ex))
                    throw new CacheException(ex);
            }
        }

        public override void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            try
            {
                if (Get(key.ToString()) != null)
                {
                    _cache.Remove(_regionName, key.ToString());
                }
            }
            catch (DataCacheException ex)
            {
                // TODO: SHould anything else happen here?
                if (!IsSafeToIgnore(ex))
                    throw new CacheException(ex);
            }
        }

        public override void Unlock(object key)
        {
            try
            {
                if (_locks.ContainsKey(key.ToString()))
                {
                    _cache.Unlock(key.ToString(), _locks[key.ToString()], _regionName);
                    _locks.Remove(key.ToString());
                }
            }
            catch (DataCacheException ex)
            {
                // TODO: ?????
                throw new CacheException(ex);
            }
        }

        private bool IsSafeToIgnore(DataCacheException ex)
        {
            return ex.ErrorCode == DataCacheErrorCode.ConnectionTerminated || ex.ErrorCode == DataCacheErrorCode.RetryLater || ex.ErrorCode == DataCacheErrorCode.Timeout;
        }

        #endregion
    }
}
