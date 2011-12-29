using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Mappings
{
    public class OrderLineMapping : Mapping<OrderLine>
    {
        #region Constructor

        public OrderLineMapping()
        {
            Id(l => l.Id, map => map.Generator(Generators.GuidComb));

            ManyToOne(p => p.Product, map =>
            {
                map.Column("ProductId");
                map.NotNullable(true);
                map.Lazy(LazyRelation.Proxy);
            });
            Property(l => l.Quantity);

            Cache(map =>
            {
                map.Include(CacheInclude.All);
                map.Region(VolatileRegion);
                map.Usage(CacheUsage.NonstrictReadWrite);
            });
        }

        #endregion
    }
}
