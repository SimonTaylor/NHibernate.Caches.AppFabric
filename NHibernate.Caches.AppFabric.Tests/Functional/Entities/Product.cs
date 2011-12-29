using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Entities
{
    public class Product
    {
        #region Properties

        public virtual Guid Id
        {
            get;
            protected internal set;
        }

        public virtual Guid ProductCode
        {
            get;
            set;
        }

        public virtual string SupplierCode
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual float Price
        {
            get;
            set;
        }

        public virtual float Height
        {
            get;
            set;
        }

        public virtual float Width
        {
            get;
            set;
        }

        public virtual float Depth
        {
            get;
            set;
        }

        public virtual float Weight
        {
            get;
            set;
        }

        #endregion
    }
}
