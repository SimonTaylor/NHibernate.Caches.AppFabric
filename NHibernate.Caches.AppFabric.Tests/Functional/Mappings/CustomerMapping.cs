using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Mappings
{
    public class CustomerMapping : Mapping<Customer>
    {
        #region Constructor

        public CustomerMapping()
        {
            Id(c => c.Id, map => map.Generator(Generators.GuidComb));

            Property(c => c.Name, map =>
            {
                map.Length(50);
                map.NotNullable(true);
            });
            Property(c => c.AddressLine1, map =>
            {
                map.Length(50);
                map.NotNullable(true);
            });
            Property(c => c.AddressLine2, map => map.Length(50));
            Property(c => c.City, map =>
            {
                map.Length(50);
                map.NotNullable(true);
            });
            Property(c => c.PostCode, map => map.Length(10));

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
