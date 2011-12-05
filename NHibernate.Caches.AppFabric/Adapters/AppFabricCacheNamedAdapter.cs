using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric.Adapters
{
    public class AppFabricCacheNamedAdapter : AppFabricCacheAdapter
    {
        #region Constructor

        public AppFabricCacheNamedAdapter(string regionName, 
                                          IDictionary<string, string> properties)
            : base(regionName, properties)
        { }

        #endregion

        #region Methods

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override object Get(object key)
        {
            throw new NotImplementedException();
        }

        public override void Lock(object key)
        {
            throw new NotImplementedException();
        }

        public override void Put(object key, object value)
        {
            throw new NotImplementedException();
        }

        public override void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public override void Unlock(object key)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
