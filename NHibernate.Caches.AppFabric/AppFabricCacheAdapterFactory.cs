using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Caches.AppFabric.Adapters;

namespace NHibernate.Caches.AppFabric
{
    public static class AppFabricCacheAdapterFactory
    {
        #region Class variables

        private static AppFabricCacheAdapterType DefaultAdapterType = AppFabricCacheAdapterType.Region;

        #endregion

        #region Methods

        /// <summary>
        /// Factory method to create an app fabric cache adapter.
        /// </summary>
        /// <param name="regionName">The name of the AppFabric cache or region etc that the adapter will interface with.</param>
        /// <param name="properties">NHibernate configuration properties, which will optionally include the type of adapter 
        /// to create.</param>
        /// <returns>An AppFabric cache adapter.</returns>
        public static AppFabricCacheAdapter Create(string regionName, IDictionary<string, string> properties)
        {
            switch (GetAdapterTypeOrDefault(properties))
            {
                case AppFabricCacheAdapterType.Named:
                    return new AppFabricCacheNamedAdapter(regionName, properties);

                case AppFabricCacheAdapterType.Region:
                    return new AppFabricCacheRegionAdapter(regionName, properties);

                case AppFabricCacheAdapterType.Tag:
                    return new AppFabricCacheTagAdapter(regionName, properties);

                default:
                    throw new HibernateException("Unknown AppFabric cache adapter type");
            }
        }

        /// <summary>
        /// Parses the configuration properties, to get the adapter type to use or simply returns the default type
        /// if one isn't provided.
        /// </summary>
        /// <param name="properties">The configuration properties.</param>
        /// <returns>The adapter type.</returns>
        private static AppFabricCacheAdapterType GetAdapterTypeOrDefault(IDictionary<string, string> properties)
        {
            AppFabricCacheAdapterType type = DefaultAdapterType;

            if (properties.ContainsKey(AppFabricConfig.NamedCacheType))
            {
                if (!Enum.TryParse<AppFabricCacheAdapterType>(properties[AppFabricConfig.NamedCacheType], out type))
                {
                    throw new HibernateException(string.Format("Invalid AppFabric provider config. If set, {0} must be set to {1}, {2} or {3}",
                                                               AppFabricConfig.NamedCacheType, AppFabricCacheAdapterType.Named, 
                                                               AppFabricCacheAdapterType.Region, AppFabricCacheAdapterType.Tag));
                }
            }
            return type;
        }

        #endregion
    }
}
