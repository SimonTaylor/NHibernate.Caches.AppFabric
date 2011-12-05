using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;

namespace NHibernate.Caches.AppFabric.Adapters
{
    public class AppFabricCacheRegionAdapter : AppFabricCacheAdapter
    {
        #region Member variable

        private readonly string _regionName;

        // TODO: This needs to be moved into the distributed cache
        private IDictionary<string, DataCacheLockHandle> _locks = new Dictionary<string,DataCacheLockHandle>();

        #endregion

        #region Constructor

        public AppFabricCacheRegionAdapter(string regionName,
                                           IDictionary<string, string> properties)
            : base(regionName, properties)
        {
            // TODO: Need to be able to recreate this if the cache cluster is restarted - think this will require specific
            // code in each method
            _regionName = regionName;

            try
            {
                Cache.CreateRegion(regionName);
            }
            catch (DataCacheException)
            {
                // TODO: Log it and swallow it if it is because it already exists?
                throw;
            }
        }

        #endregion

        #region Methods

        public override void Clear()
        {
            Cache.ClearRegion(_regionName);
        }

        public override void Destroy()
        {
            Clear();
        }

        public override object Get(object key)
        {
            if (key == null)
                return null;

            return Cache.Get(key.ToString(), _regionName);
        }

        public override void Lock(object key)
        {
            DataCacheLockHandle lockHandle = null;

            try
            {
                Cache.GetAndLock(key.ToString(), TimeSpan.FromMilliseconds(Timeout), out lockHandle, _regionName);
                _locks.Add(key.ToString(), lockHandle);
            }
            catch (DataCacheException) 
            { 
                // TODO: ??? WHat should go here???
            }
        }

        public override void Put(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key", "null key not allowed");

            if (value == null)
                throw new ArgumentNullException("value", "null value not allowed");

            // TODO: Should we check for locks?

            Cache.Put(key.ToString(), value, _regionName);
        }

        public override void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (Get(key.ToString()) != null)
            {
                Cache.Remove(_regionName, key.ToString());
            }
        }

        public override void Unlock(object key)
        {
            try
            {
                if (_locks.ContainsKey(key.ToString()))
                {
                    Cache.Unlock(key.ToString(), _locks[key.ToString()], _regionName);
                    _locks.Remove(key.ToString());
                }
            }
            catch (DataCacheException) 
            { 
                // TODO: ?????
            }
        }

        #endregion
    }
}
