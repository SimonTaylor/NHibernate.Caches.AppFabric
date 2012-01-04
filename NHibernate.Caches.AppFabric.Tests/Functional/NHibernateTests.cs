#region License

//Microsoft Public License (Ms-PL)
//
//This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
//
//1. Definitions
//
//The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
//
//A "contribution" is the original software, or any additions or changes to the software.
//
//A "contributor" is any person that distributes its contribution under this license.
//
//"Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
//2. Grant of Rights
//
//(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
//
//(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
//
//3. Conditions and Limitations
//
//(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
//
//(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
//
//(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
//
//(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
//
//(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

#endregion

using System;
using System.Collections.Generic;
using NHibernate.Caches.AppFabric.Tests.Common;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;
using NUnit.Framework;

namespace NHibernate.Caches.AppFabric.Tests.Functional
{
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
