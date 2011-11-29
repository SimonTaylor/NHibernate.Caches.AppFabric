using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cache;

namespace NHibernate.Caches.AppFabric
{
    public abstract class AppFabricCacheAdapter : ICache
    {
        #region Constants

        private const string TimeoutSetting = "cache.app-fabric.timeout";
        private const string NamedCacheType = "cache.app-fabric.cache-type";


        private int DefaultTimeout = 30;

        #endregion

        #region Constructor

        public AppFabricCacheAdapter(string regionName, IDictionary<string, string> properties)
        {
            // TODO: I don't think this can be null, should probably check though - this is the name of the cache
            RegionName = regionName;

            // TODO: need to tidy this up for other settings, need to handle parse errors ??? default and log ????
            // Is 30 an OK default setting?
            if (properties.ContainsKey(TimeoutSetting))
                Timeout = Int32.Parse(properties[TimeoutSetting]);
            else
            {
                Timeout = DefaultTimeout;
            }

            // Maybe the properties should be passed to the factory and then when the type is parsed, 
            // the rest of the properties can be passed to the constructor, along with region name
            // The parsing of the properties can go in the constructor of the abstract base class
            if (properties.ContainsKey(NamedCacheType))
                Enum.Parse(typeof(AppFabricAdapterType), properties[NamedCacheType]); // Pass to factory
            else
            {
                // Just do the default
            }

            // Cache client config should have default versions and then specific ones. i.e. Everything could use the
            // one that is configured through web.config except any that are explictly set where the name of the client
            // matches the region name
            // Creating the factories are expensive, so I should cache them in a static dictionary which I can key with
            // the region name
            // Cache type will be Tag, Region, Named - that will be passed to a factory that will create my different
            // things - need to think of a name for them.

            // TODO: All of the classes etc need file headers
        }

        #endregion

        #region Properties

        public string RegionName
        {
            get;
            private set;
        }

        public int Timeout
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public object Get(object key)
        {
            throw new NotImplementedException();
        }

        public void Lock(object key)
        {
            throw new NotImplementedException();
        }

        public long NextTimestamp()
        {
            throw new NotImplementedException();
        }

        public void Put(object key, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public void Unlock(object key)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
