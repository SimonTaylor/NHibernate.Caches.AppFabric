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
        #region Member variables

        private readonly string _regionName;

        #endregion

        #region Constructor

        public AppFabricCacheRegionAdapter(string regionName)
            : base(regionName)
        {
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

        protected internal override DataCache GetCache(IAppFabricCacheFactory cacheFactory)
        {
            return cacheFactory.GetCache(AppFabricProviderSettings.Settings.RegionCacheTypeCacheName, true);
        }

        #endregion
    }
}
