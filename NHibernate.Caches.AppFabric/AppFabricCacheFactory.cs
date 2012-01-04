using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;

namespace NHibernate.Caches.AppFabric
{
    public class AppFabricCacheFactory : IAppFabricCacheFactory
    {
        #region Class variables

        private static readonly Lazy<AppFabricCacheFactory> _lazy = new Lazy<AppFabricCacheFactory>(() => new AppFabricCacheFactory());

        #endregion

        #region Member variables

        private DataCacheFactory _cacheCluster;

        #endregion

        #region Constructor

        private AppFabricCacheFactory()
        {
            _cacheCluster = new DataCacheFactory();
        }

        #endregion

        #region Properties

        public static AppFabricCacheFactory Instance
        {
            get
            {
                return _lazy.Value;
            }
        }

        #endregion

        #region Methods

        public DataCache GetCache(string cacheName, bool useDefault = false)
        {
            try
            {
                return _cacheCluster.GetCache(cacheName);
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.NamedCacheDoesNotExist && useDefault)
                    return _cacheCluster.GetDefaultCache();

                throw;
            }
        }

        #endregion
    }
}
