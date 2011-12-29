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

        public DataCache GetCache(string cacheName, bool useDefault)
        {
            try
            {
                return _cacheCluster.GetCache(cacheName);
            }
            catch (DataCacheException ex)
            {
                // TODO: Whether this is done is dependent upong the implementation of the adapter so should be determined
                // by a flag passed in
                if (ex.ErrorCode == DataCacheErrorCode.NamedCacheDoesNotExist && useDefault)
                    return _cacheCluster.GetDefaultCache();

                throw;
            }
        }

        #endregion
    }
}
