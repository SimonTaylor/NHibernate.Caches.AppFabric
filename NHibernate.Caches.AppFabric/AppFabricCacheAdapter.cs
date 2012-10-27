#region License

//Microsoft Public License (Ms-PL)
//
//This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
//
//1. Definitions
//
//The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
//
//A "contribution" is the original software, or any additions or changes to the software.
//
//A "contributor" is any person that distributes its contribution under this license.
//
//"Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
//2. Grant of Rights
//
//(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
//
//(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
//
//3. Conditions and Limitations
//
//(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
//
//(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
//
//(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
//
//(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
//
//(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

#endregion

using System;
using System.Collections.Generic;
using Microsoft.ApplicationServer.Caching;
using NHibernate.Cache;
using NHibernate.Util;

namespace NHibernate.Caches.AppFabric
{
    /// <summary>
    /// An AppFabric implementation of <see cref="ICache"/>.
    /// The locking functionality in here makes a couple of assumptions. Firstly that NHibernate will call lock/unlock
    /// appropriately and that the provider code does not need to ensure that items are locked before putting or removing
    /// etc. Secondly, it assumes that in a distributed environment all lock/unlock calls will happen in the same request 
    /// and it is therefore acceptable to cache the lock handles in memory.
    /// </summary>
    public abstract class AppFabricCacheAdapter : ICache
    {
        #region Member variables

        private readonly ISerializationProvider _serializationProvider;

        private readonly IDictionary<string, DataCacheLockHandle> _lockHandles;
        private readonly DataCache _locksCache;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="regionName">The name of the NHibernate region the adapter is for.</param>
        public AppFabricCacheAdapter(string regionName)
        {
            RegionName = regionName;

            if (!string.IsNullOrEmpty(AppFabricProviderSettings.Settings.SerializationProvider))
                _serializationProvider = Activator.CreateInstance(ReflectHelper.ClassForName(AppFabricProviderSettings.Settings.SerializationProvider)) as ISerializationProvider;
            else
            {
                _serializationProvider = null;
            }
            _lockHandles = new Dictionary<string, DataCacheLockHandle>();
            _locksCache  = AppFabricCacheFactory.Instance.GetCache(LocksRegionName, true);

            Cache = GetCache(AppFabricCacheFactory.Instance);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the region to cache the data.
        /// </summary>
        protected internal abstract string AppFabricRegionName
        {
            get;
        }

        /// <summary>
        /// The name of the region and the cache to store the locks - in reality it performs the lock in this region,
        /// rather than storing the locks in it.
        /// </summary>
        protected internal virtual string LocksRegionName
        {
            get
            {
                return AppFabricProviderSettings.Settings.LocksRegionName;
            }
        }

        /// <summary>
        /// The data cache [client] for this adapter.
        /// </summary>
        protected internal virtual DataCache Cache
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the [NHibernate] cache region.
        /// </summary>
        public virtual string RegionName
        {
            get;
            private set;
        }

        /// <summary>
        /// Get a reasonable "lock timeout"
        /// </summary>
        public virtual int Timeout
        {
            get
            {
                return AppFabricProviderSettings.Settings.LockTimeout;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a data cache [client] for this adapter.
        /// </summary>
        /// <param name="cacheFactory">A cache factory used for creating data cache [clients].</param>
        /// <returns>A data cache [client].</returns>
        protected internal abstract DataCache GetCache(IAppFabricCacheFactory cacheFactory);

        /// <summary>
        /// Clear the Cache
        /// </summary>
        /// <exception cref="CacheException"></exception>
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

        /// <summary>
        /// Clean up.
        /// </summary>
        /// <exception cref="CacheException"></exception>
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

        /// <summary>
        /// Get the object from the Cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// If this is a clustered cache, lock the item
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to lock.</param>
        /// <exception cref="CacheException"></exception>
        public virtual void Lock(object key)
        {
            Lock(key, true);
        }

        /// <summary>
        /// Attempts to lock the key in the locks region and stores a reference to the lock handle in memory.
        /// If the lock region doesn't exists, it will attempt to create it.
        /// </summary>
        /// <param name="key">The key to lock.</param>
        /// <param name="createMissingRegion">A flag to determine whether or not to create the lock region if it doesn't
        /// exist.</param>
        private void Lock(object key, bool createMissingRegion)
        {
            DataCacheLockHandle lockHandle = null;

            try
            {
                _locksCache.GetAndLock(key.ToString(), TimeSpan.FromMilliseconds(Timeout), out lockHandle, LocksRegionName, true);

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

        /// <summary>
        /// Generate a timestamp
        /// </summary>
        /// <returns></returns>
        public virtual long NextTimestamp()
        {
            return Timestamper.Next();
        }

        /// <summary>
        /// Caches an item.
        /// </summary>
        /// <param name="key">The key for the item to cache.</param>
        /// <param name="value">The item to cache.</param>
        public virtual void Put(object key, object value)
        {
            Put(key, value, true);
        }

        /// <summary>
        /// Caches an item. If a serialization provider has been configured that will be used to serialize the value
        /// rather than relying on AppFanbric's default implementation. If the region the item is to be cached in doesn't
        /// exists, ite will be created.
        /// </summary>
        /// <param name="key">The key for the item to be cached.</param>
        /// <param name="value">The item to be cached.</param>
        /// <param name="createMissingRegion">A flag to determine whether or not the cache region should be created if it
        /// doesn't exist.</param>
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

        /// <summary>
        /// Remove an item from the Cache.
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to remove.</param>
        /// <exception cref="CacheException"></exception>
        public virtual void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            try
            {
                if (Get(key.ToString()) != null)
                {
                    Cache.Remove(key.ToString(),AppFabricRegionName);
                }
            }
            catch (DataCacheException ex)
            {
                if (!IsSafeToIgnore(ex) && ex.ErrorCode != DataCacheErrorCode.RegionDoesNotExist)
                    throw new CacheException(ex);
            }
        }

        /// <summary>
        /// If this is a clustered cache, unlock the item
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to unlock.</param>
        /// <exception cref="CacheException"></exception>
        public virtual void Unlock(object key)
        {
            try
            {
                if (_lockHandles.ContainsKey(key.ToString()))
                {
                    _locksCache.Unlock(key.ToString(), _lockHandles[key.ToString()], LocksRegionName);

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

        /// <summary>
        /// Checks whether the execption thrown by the AppFabric client is safe to ignore or not. Sometime sommunication between
        /// the client and server can be slow for example in which case the client will throw na exception, but we would want to
        /// ignore it.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsSafeToIgnore(DataCacheException ex)
        {
            return ex.ErrorCode == DataCacheErrorCode.ConnectionTerminated || ex.ErrorCode == DataCacheErrorCode.RetryLater || ex.ErrorCode == DataCacheErrorCode.Timeout;
        }

        /// <summary>
        /// Creates the locks AppFabric cache region.
        /// </summary>
        /// <param name="callback">A call back to call once the region has been created.</param>
        private void CreateLocksRegion(Action<bool> callback)
        {
            CreateRegion(_locksCache, LocksRegionName, callback);
        }

        /// <summary>
        /// Creates the App Fabric cache region used for caching items.
        /// </summary>
        /// <param name="callback">A call back to call once the region has been created.</param>
        private void CreateAppFabricRegion(Action<bool> callback)
        {
            CreateRegion(Cache, AppFabricRegionName, callback);
        }

        /// <summary>
        /// Creates an AppFabric cache region.
        /// </summary>
        /// <param name="cache">The data cache [client] to use to create the cache region.</param>
        /// <param name="regionName">The name of the AppFabric cache region to create.</param>
        /// <param name="callback">A call back to call once the region has been created.</param>
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
