using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;
using NHibernate.Cache;

namespace NHibernate.Caches.AppFabric.Adapters
{
    public class AppFabricCacheNamedAdapter : AppFabricCacheAdapter
    {
        #region Constants

        private const string DefaultRegionName = "nhibernate";

        #endregion

        #region Constructor

        public AppFabricCacheNamedAdapter(string regionName,
                                          IDictionary<string, string> properties)
            : base(regionName, properties)
        {
            // TODO: Is extra error handling required here?
        }

        #endregion

        #region Properties

        protected internal override string AppFabricRegionName
        {
            get
            {
                // Need to be able to get this from config
                return DefaultRegionName;
            }
        }

        #endregion

        #region Methods

        protected internal override DataCache GetCache(IAppFabricCacheFactory cacheFactory, IDictionary<string, string> properties)
        {
            // TODO: Do we need to garuntee the size of the region name?
            // TODO: When using named caches, do we really want to default to the default cache? Without doing this we have to 
            // create named caches for the randomly named nhibernate ones. This should probably be configurable
            return cacheFactory.GetCache(RegionName, true);
        }

        #endregion
    }
}
