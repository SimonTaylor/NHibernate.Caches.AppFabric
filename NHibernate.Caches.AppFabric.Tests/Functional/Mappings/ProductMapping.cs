using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Mappings
{
    public class ProductMapping : Mapping<Product>
    {
        #region Constructor

        public ProductMapping()
        {
            Id(p => p.Id, map => map.Generator(Generators.GuidComb));

            Property(p => p.ProductCode);
            Property(p => p.SupplierCode, map =>
            {
                map.Length(25);
                map.NotNullable(true);
            });
            Property(p => p.Description, map =>
            {
                map.Length(512);
                map.NotNullable(true);
            });
            Property(p => p.Price);
            Property(p => p.Height);
            Property(p => p.Width);
            Property(p => p.Depth);
            Property(p => p.Weight);

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
