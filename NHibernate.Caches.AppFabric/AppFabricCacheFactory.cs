using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;

namespace NHibernate.Caches.AppFabric
{
    public class AppFabricCacheFactory : IAppFabricCacheFactory
    {
        #region Member variables    

        private DataCacheFactory _cacheCluster;

        #endregion

        #region Constructor

        public AppFabricCacheFactory(IDictionary<string, string> properties)
        {
            // TODO: SHould this be static - maybe doesn't matter if it gets added to a static dictionary?
            // Or maybe this class should be a singleton? And what about disposing of it?
            _cacheCluster = new DataCacheFactory();
        }

        #endregion

        #region Methods

        public DataCache GetCache(string cacheName)
        {
            try
            {
                return _cacheCluster.GetCache(cacheName);
            }
            catch (DataCacheException ex)
            {
                // TODO: Is this the right thing to do. Maybe it depends on what has been configured
                if (ex.ErrorCode == DataCacheErrorCode.CacheAdminCacheNotCreated)
                    return _cacheCluster.GetDefaultCache();

                throw;
            }
        }

        #endregion
    }
}
