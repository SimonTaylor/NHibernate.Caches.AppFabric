using System;
using Microsoft.ApplicationServer.Caching;

namespace NHibernate.Caches.AppFabric
{
    public interface IAppFabricCacheFactory
    {
        DataCache GetCache(string cacheName, bool useDefault);
    }
}
