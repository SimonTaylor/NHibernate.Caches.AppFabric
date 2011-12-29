using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric.Tests.Functional.Entities
{
    public class Order
    {
        #region Member variables

        private IList<OrderLine> _orderLines;

        #endregion

        #region Properties

        public virtual Guid Id
        {
            get;
            protected internal set;
        }

        public virtual Customer Customer
        {
            get;
            set;
        }

        public virtual IList<OrderLine> OrderLines
        {
            get
            {
                return _orderLines ?? (_orderLines = new List<OrderLine>());
            }
            set
            {
                _orderLines = value;
            }
        }

        #endregion
    }
}
