using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Entities
{
    public class OrderLine
    {
        #region Properties

        public virtual Guid Id
        {
            get;
            protected internal set;
        }

        public virtual Order Order
        {
            get;
            set;
        }

        public virtual Product Product
        {
            get;
            set;
        }

        public virtual int Quantity
        {
            get;
            set;
        }

        #endregion
    }
}
