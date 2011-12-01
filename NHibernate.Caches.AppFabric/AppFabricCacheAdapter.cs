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

        private int DefaultTimeout = 30;

        #endregion

        #region Constructor

        public AppFabricCacheAdapter(string regionName, IDictionary<string, string> properties)
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

            // Cache client config should have default versions and then specific ones. i.e. Everything could use the
            // one that is configured through web.config except any that are explictly set where the name of the client
            // matches the region name
            // Creating the factories are expensive, so I should cache them in a static dictionary which I can key with
            // the region name
            // Cache type will be Tag, Region, Named - that will be passed to a factory that will create my different
            // things - need to think of a name for them.

            // TODO: All of the classes etc need file headers
            // TODO: Need to add in logging as well
        }

        #endregion

        #region Properties

        public string RegionName
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

        public abstract void Clear();

        public abstract void Destroy();

        public abstract object Get(object key);

        public abstract void Lock(object key);

        public virtual long NextTimestamp()
        {
            return Timestamper.Next();
        }

        public abstract void Put(object key, object value);

        public abstract void Remove(object key);

        public abstract void Unlock(object key);

        #endregion
    }
}
