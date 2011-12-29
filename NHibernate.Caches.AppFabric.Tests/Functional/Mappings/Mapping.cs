using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Mappings
{
    public abstract class Mapping<T> : ClassMapping<T> where T : class
    {
        protected internal const string StaticRegion   = "static";
        protected internal const string VolatileRegion = "volatile";
    }
}
