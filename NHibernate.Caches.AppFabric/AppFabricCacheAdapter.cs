using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cache;
using Microsoft.ApplicationServer.Caching;
using NHibernate.Util;

namespace NHibernate.Caches.AppFabric
{
    /// <summary>
    /// The locking functionality in here makes a couple of assumptions. Firstly that NHibernate will call lock/unlock
    /// appropriately and that the provider code does not need to ensure that items are locked before putting or removing
    /// etc. Secondly, it assumes that a lock/unlock calls will happen in the same request and it is therefore acceptable
    /// to cache the lock handles in memory.
    /// </summary>
    public abstract class AppFabricCacheAdapter : ICache
    {
        #region Constants

        private int DefaultTimeout = 30000;
        private string LocksRegion = "locks";

        #endregion

        #region Member variables

        private readonly ISerializationProvider _serializationProvider;

        private readonly IDictionary<string, DataCacheLockHandle> _lockHandles;
        private readonly DataCache _locksCache;

        #endregion

        #region Constructor

        public AppFabricCacheAdapter(string regionName, 
                                     IDictionary<string, string> properties)
        {
            // TODO: I don't think this can be null, should probably check though - this is the name of the cache
            RegionName = regionName;

            // TODO: need to tidy this up for other settings, need to handle parse errors ??? default and log ????
            // Is 30 an OK default setting?
            if (properties.ContainsKey(AppFabricConfig.TimeoutSetting))
                Timeout = Int32.Parse(properties[AppFabricConfig.TimeoutSetting]);
            else
            {
                Timeout = DefaultTimeout;
            }

            if (properties.ContainsKey(AppFabricConfig.Serialization))
                _serializationProvider = Activator.CreateInstance(ReflectHelper.ClassForName(properties[AppFabricConfig.Serialization])) as ISerializationProvider;
            else
            {
                _serializationProvider = null;
            }

            // Cache client config should have default versions and then specific ones. i.e. Everything could use the
            // one that is configured through web.config except any that are explictly set where the name of the client
            // matches the region name
            // Creating the factories are expensive, so I should cache them in a static dictionary which I can key with
            // the region name
            // Cache type will be Tag, Region, Named - that will be passed to a factory that will create my different
            // things - need to think of a name for them.

            // TODO: All of the classes etc need file headers
            // TODO: Need to add in logging as well
            IAppFabricCacheFactory factory = new AppFabricCacheFactory(properties);

            Cache = GetCache(factory, properties);

            // TODO: Make locks region / named cache configurable
            _locksCache = factory.GetCache(LocksRegion, true);
            _lockHandles      = new Dictionary<string, DataCacheLockHandle>();
        }

        #endregion

        #region Properties

        protected internal abstract string AppFabricRegionName
        {
            get;
        }

        protected internal virtual DataCache Cache
        {
            get;
            set;
        }

        public virtual string RegionName
        {
            get;
            private set;
        }

        public virtual int Timeout
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        protected internal abstract DataCache GetCache(IAppFabricCacheFactory cacheFactory, IDictionary<string, string> properties);

        public virtual void Clear()
        {
            try
            {
                Cache.ClearRegion(AppFabricRegionName);
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode != DataCacheErrorCode.RegionDoesNotExist)
                    throw new CacheException(ex);
            }
        }

        public virtual void Destroy()
        {
            try
            {
                Cache.RemoveRegion(AppFabricRegionName);
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode != DataCacheErrorCode.RegionDoesNotExist)
                    throw new CacheException(ex);
            }
        }

        public virtual object Get(object key)
        {
            if (key == null)
                return null;

            try
            {
                object value = Cache.Get(key.ToString(), AppFabricRegionName);

                if (_serializationProvider != null && value != null)
                    value = _serializationProvider.Deserialize((byte[])value);

                return value;
            }
            catch (DataCacheException ex)
            {
                if (IsSafeToIgnore(ex) || ex.ErrorCode == DataCacheErrorCode.RegionDoesNotExist)
                    return null;
                else
                {
                    throw new CacheException(ex);
                }
            }
        }

        public virtual void Lock(object key)
        {
            Lock(key, true);
        }

        private void Lock(object key, bool createMissingRegion)
        {
            DataCacheLockHandle lockHandle = null;

            try
            {
                _locksCache.GetAndLock(key.ToString(), TimeSpan.FromMilliseconds(Timeout), out lockHandle, LocksRegion, true);

                lock (_lockHandles)
                {
                    _lockHandles.Add(key.ToString(), lockHandle);
                }
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.RegionDoesNotExist && createMissingRegion)
                {
                    CreateLocksRegion(b => Lock(key, false));
                }
                else
                {
                    throw new CacheException(ex);
                }
            }
        }

        public virtual long NextTimestamp()
        {
            return Timestamper.Next();
        }

        public virtual void Put(object key, object value)
        {
            Put(key, value, true);
        }

        private void Put(object key, object value, bool createMissingRegion)
        {
            if (key == null)
                throw new ArgumentNullException("key", "null key not allowed");

            if (value == null)
                throw new ArgumentNullException("value", "null value not allowed");

            try
            {
                if (_serializationProvider != null)
                    value = _serializationProvider.Serialize(value);

                Cache.Put(key.ToString(), value, AppFabricRegionName);
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.RegionDoesNotExist && createMissingRegion)
                    CreateAppFabricRegion(b => Put(key, value, false));
                else if (!IsSafeToIgnore(ex) && ex.ErrorCode != DataCacheErrorCode.ObjectLocked)
                {
                    throw new CacheException(ex);
                }
            }
        }

        public virtual void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            try
            {
                if (Get(key.ToString()) != null)
                {
                    Cache.Remove(AppFabricRegionName, key.ToString());
                }
            }
            catch (DataCacheException ex)
            {
                if (!IsSafeToIgnore(ex) && ex.ErrorCode != DataCacheErrorCode.RegionDoesNotExist)
                    throw new CacheException(ex);
            }
        }

        public virtual void Unlock(object key)
        {
            try
            {
                if (_lockHandles.ContainsKey(key.ToString()))
                {
                    _locksCache.Unlock(key.ToString(), _lockHandles[key.ToString()], LocksRegion);

                    lock (_lockHandles)
                    {
                        _lockHandles.Remove(key.ToString());
                    }
                }
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode != DataCacheErrorCode.RegionDoesNotExist)
                    throw new CacheException(ex);
            }
        }

        private bool IsSafeToIgnore(DataCacheException ex)
        {
            return ex.ErrorCode == DataCacheErrorCode.ConnectionTerminated || ex.ErrorCode == DataCacheErrorCode.RetryLater || ex.ErrorCode == DataCacheErrorCode.Timeout;
        }

        private void CreateLocksRegion(Action<bool> callback)
        {
            CreateRegion(_locksCache, LocksRegion, callback);
        }

        private void CreateAppFabricRegion(Action<bool> callback)
        {
            CreateRegion(Cache, AppFabricRegionName, callback);
        }

        private void CreateRegion(DataCache cache, string regionName, Action<bool> callback)
        {
            try
            {
                callback(cache.CreateRegion(regionName));
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.RegionAlreadyExists)
                    callback(false);
                else
                {
                    throw new CacheException(ex);
                }
            }
        }

        #endregion
    }
}
