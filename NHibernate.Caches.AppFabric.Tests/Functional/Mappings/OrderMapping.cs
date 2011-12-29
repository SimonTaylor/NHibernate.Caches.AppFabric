using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Mappings
{
    public class OrderMapping : Mapping<Order>
    {
        #region Constructor

        public OrderMapping()
        {
            Id(o => o.Id, map => map.Generator(Generators.GuidComb));

            ManyToOne(o => o.Customer, map =>
            {
                map.Column("CustomerId");
                map.NotNullable(true);
                map.Lazy(LazyRelation.Proxy);
            });
            Bag(o => o.OrderLines, map =>
            {
                map.Inverse(false);
                map.Cascade(Cascade.All);
                map.Lazy(CollectionLazy.Lazy);
            },
            mapping => mapping.OneToMany());

            Cache(map =>
            {
                map.Include(CacheInclude.All);
                map.Region(StaticRegion);
                map.Usage(CacheUsage.ReadOnly);
            });
        }

        #endregion
    }
}
