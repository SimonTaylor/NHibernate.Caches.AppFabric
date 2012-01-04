﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cache;

namespace NHibernate.Caches.AppFabric
{
    // TODO: COnvert to .NET 3.5 or have 4 and 3.5 builds?
    public class AppFabricProvider : ICacheProvider
    {
        public ICache BuildCache(string regionName, IDictionary<string, string> properties)
        {
            return AppFabricCacheAdapterFactory.Create(regionName, properties);
        }

        public long NextTimestamp()
        {
            return Timestamper.Next();
        }

        public void Start(IDictionary<string, string> properties)
        {
            // TODO: Read this from App.config properly
            properties.Add("cache.app-fabric.cache-type", "Named");
            properties.Add("cache.app-fabric.serialization_provider", "NHibernate.Caches.AppFabric.Tests.Functional.SerializationProvider, NHibernate.Caches.AppFabric.Tests");
        }

        public void Stop()
        {
        }
    }
}
