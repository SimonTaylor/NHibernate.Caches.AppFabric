using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cache;
using System.Configuration;

namespace NHibernate.Caches.AppFabric
{
    public class AppFabricProvider : ICacheProvider
    {
        public ICache BuildCache(string regionName, IDictionary<string, string> properties)
        {
            return AppFabricCacheAdapterFactory.Create(regionName);
        }

        public long NextTimestamp()
        {
            return Timestamper.Next();
        }

        public void Start(IDictionary<string, string> properties)
        {
            if (AppFabricProviderSettings.Settings == null)
                throw new ConfigurationErrorsException("Missing required AppFabricProvider configuration section");
        }

        public void Stop()
        {
        }
    }
}
