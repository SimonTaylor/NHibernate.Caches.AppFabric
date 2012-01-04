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

        #region Constructor

        public AppFabricCacheNamedAdapter(string regionName)
            : base(regionName)
        {
        }

        #endregion

        #region Properties

        protected internal override string AppFabricRegionName
        {
            get
            {
                return AppFabricProviderSettings.Settings.NamedCacheTypeRegionName;
            }
        }

        #endregion

        #region Methods

        protected internal override DataCache GetCache(IAppFabricCacheFactory cacheFactory)
        {
            return cacheFactory.GetCache(RegionName, !AppFabricProviderSettings.Settings.NamedCachesMustExist);
        }

        #endregion
    }
}
