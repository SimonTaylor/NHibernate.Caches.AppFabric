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

        #endregion

        #region Constructor

        public AppFabricCacheRegionAdapter(string regionName,
                                           IDictionary<string, string> properties)
            : base(regionName, properties)
        {
            // TODO: Need to be able to recreate this if the cache cluster is restarted - think this will require specific
            // code in each method
            _regionName = regionName.GetHashCode().ToString();
        }

        #endregion

        #region Properties

        protected internal override string AppFabricRegionName
        {
            get
            {
                return _regionName;
            }
        }

        #endregion

        #region Methods

        protected internal override DataCache GetCache(IAppFabricCacheFactory cacheFactory, IDictionary<string, string> properties)
        {
            // TODO: Get the cache name from the config if it exists - is extra error handling required here?
            return cacheFactory.GetCache(DefaultCacheName, true);
        }

        #endregion
    }
}
