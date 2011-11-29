using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric
{
    public class AppFabricCacheAdapterFactory
    {
        private static AppFabricCacheAdapterType DefaultAdapterType = AppFabricCacheAdapterType.Region;

        public static AppFabricCacheAdapter Create(string regionName, IDictionary<string, string> properties)
        {
            AppFabricCacheAdapterType type = DefaultAdapterType;

            // Need to parse the type and then instantiate one of the adapter types.
            // Need to log errors etc if they occur, using standard NHibernate logging
            
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

            if (properties.ContainsKey("Need to put properties in a constants file instead"))
            {
                if (!Enum.TryParse<AppFabricCacheAdapterType>(properties["TODO"], out type))
                    throw new NHibernate.HibernateException("TODO include key and possible values in error");
            }
            return type;
        }
    }
}
