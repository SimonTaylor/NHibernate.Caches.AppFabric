using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate.Caches.AppFabric.Tests.Common;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;

namespace NHibernate.Caches.AppFabric.Tests.Functional
{
    // TODO: Could do with functional tests that hit the other methods, these currently only hit put and get
    [TestFixture]
    public class NHibernateTests
    {
        #region Setup / Teardown

        [SetUp]
        public void SetUp()
        {
            NHibernateSessionManager.OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            NHibernateSessionManager.CloseSession();
        }

        #endregion

        #region Helper properties / methods

        private ISession Session
        {
            get
            {
                return NHibernateSessionManager.CurrentSession;
            }
        }

        private IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return Session.CreateCriteria<TEntity>().List<TEntity>();
        }

        #endregion

        #region Tests

        [Test]
        public void CanGetCachedEntityById()
        {
            // Arrange - read once to seed the second level cache and close and open a new session
            IList<Product> all = GetAll<Product>();

            Product p = Session.Get<Product>(all[0].Id);

            NHibernateSessionManager.CloseSession();
            NHibernateSessionManager.OpenSession();

            // Act - This should come from the second level cache this time
            Product p2 = Session.Get<Product>(all[0].Id);

            // Assert - just make sure something is returned, we're not testing NHibernate
            Assert.IsNotNull(p2);
        }

        [Test]
        public void CanGetCachedChildren()
        {
            // Arrange - read once to seed the second level cache and and close and open a new session
            IList<Order> all = GetAll<Order>();

            Order o = Session.Get<Order>(all[0].Id);
            Product p = o.OrderLines[0].Product;

            NHibernateSessionManager.CloseSession();
            NHibernateSessionManager.OpenSession();

            // Act - This should all come from the second level cache this time
            Order o2 = Session.Get<Order>(all[0].Id);
            Product p2 = o.OrderLines[0].Product;

            // Assert - just make sure something is returned, we're not testing NHibernate
            Assert.IsNotNull(p2);
            Assert.AreNotEqual(Guid.Empty, p2.Id);
        }

        [Test]
        public void CanGetCachedQueryResults()
        {
            // Arrange - read once to seed the second level cache
            IList<Product> all = GetAll<Product>();

            IList<Order> orders = CreateOrdersByProductQuery(all[0].Id).List<Order>();

            NHibernateSessionManager.CloseSession();
            NHibernateSessionManager.OpenSession();

            // Act - This should come from the second level cache this time
            IList<Order> orders2 = CreateOrdersByProductQuery(all[0].Id).List<Order>();

            // Assert - just make sure something is returned, we're not teststing NHibernate
            Assert.IsNotNull(orders2);
        }

        private IQueryOver<Order> CreateOrdersByProductQuery(Guid productId)
        {
            #region Aliases

            OrderLine line = null;

            #endregion

            // Assumes there will not be multiple order lines for the same product
            return Session.QueryOver<Order>()
                          .JoinAlias(o => o.OrderLines, () => line)
                          .Where(() => line.Product.Id == productId)
                          .Cacheable()
                          .CacheRegion("volatile");
        }

        #endregion

    }
}
