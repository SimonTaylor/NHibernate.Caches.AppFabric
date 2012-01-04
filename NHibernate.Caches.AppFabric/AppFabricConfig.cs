using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric
{
    public static class AppFabricConfig
    {
        internal const string TimeoutSetting = "cache.app-fabric.timeout";
        internal const string NamedCacheType = "cache.app-fabric.cache-type";
        internal const string Serialization  = "cache.app-fabric.serialization_provider";
    }
}
