using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Entities
{
    public class Customer
    {
        #region Properties

        public virtual Guid Id
        {
            get;
            protected internal set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string AddressLine1
        {
            get;
            set;
        }

        public virtual string AddressLine2
        {
            get;
            set;
        }

        public virtual string City
        {
            get;
            set;
        }

        public virtual string PostCode
        {
            get;
            set;
        }

        #endregion
    }
}
