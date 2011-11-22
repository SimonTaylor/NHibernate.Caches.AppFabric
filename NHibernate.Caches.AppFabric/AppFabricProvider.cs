using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cache;

namespace NHibernate.Caches.AppFabric
{
    public class AppFabricProvider : ICacheProvider
    {
        public ICache BuildCache(string regionName, IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public long NextTimestamp()
        {
            throw new NotImplementedException();
        }

        public void Start(IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
