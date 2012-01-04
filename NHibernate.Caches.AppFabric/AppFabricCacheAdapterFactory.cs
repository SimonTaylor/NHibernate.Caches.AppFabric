using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Caches.AppFabric.Adapters;

namespace NHibernate.Caches.AppFabric
{
    public static class AppFabricCacheAdapterFactory
    {
        #region Methods

        /// <summary>
        /// Factory method to create an app fabric cache adapter.
        /// </summary>
        /// <param name="regionName">The name of the AppFabric cache or region etc that the adapter will interface with.</param>
        /// <returns>An AppFabric cache adapter.</returns>
        public static AppFabricCacheAdapter Create(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
                throw new ArgumentNullException("A region name must be specified");

            switch (AppFabricProviderSettings.Settings.CacheType)
            {
                case AppFabricCacheAdapterType.Named:
                    return new AppFabricCacheNamedAdapter(regionName);

                case AppFabricCacheAdapterType.Region:
                    return new AppFabricCacheRegionAdapter(regionName);

                default:
                    throw new HibernateException("Unknown AppFabric cache adapter type");
            }
        }

        #endregion
    }
}
