using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate.Caches.AppFabric.Tests.Common;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;

namespace NHibernate.Caches.AppFabric.Tests
{
    [SetUpFixture]
    public class AssemblySetup
    {
        [SetUp]
        public void Setup()
        {
            NHibernateSessionManager.Initialize();

            NHibernateSessionManager.OpenSession();
            NHibernateSessionManager.BeginTransaction();

            try
            {
                CreateStandardDataSet();
            }
            finally
            {
                NHibernateSessionManager.CommitTransaction();
                NHibernateSessionManager.CloseSession();
            }
        }

        private void CreateStandardDataSet()
        {
            Product p1 = InsertProduct(Guid.NewGuid(), "Football", "SP10001", 9.99f, 300.0f, 300.0f, 300.0f, 0.01f);
            Product p2 = InsertProduct(Guid.NewGuid(), "Goaly gloves", "SP10002", 19.99f, 10.0f, 100.0f, 150.0f, 0.005f);
            Product p3 = InsertProduct(Guid.NewGuid(), "Shin pads", "SP10003", 4.99f, 450.0f, 100.0f, 100.0f, 0.1f);

            Customer c1 = InsertCustomer("A Taylor", "1, The High Street", "England", "London", "SW1 1QR");
            Customer c2 = InsertCustomer("B Taylor", "2, The High Street", "England", "London", "SW1 1QR");
            Customer c3 = InsertCustomer("C Taylor", "3, The High Street", "England", "London", "SW1 1QR");

            InsertOrder(c1, new [] { CreateOrderLine(p1, 5), CreateOrderLine(p2, 1) });
            InsertOrder(c1, new [] { CreateOrderLine(p3, 2) });
            InsertOrder(c2, new [] { CreateOrderLine(p2, 2) });
            InsertOrder(c3, new [] { CreateOrderLine(p3, 3) });

            Session.Flush();
        }

        private Product InsertProduct(Guid productCode, string description, string supplierCode = "", float price = 0.0f,
                                      float height = 0.0f, float width = 0.0f, float depth = 0.0f, float weight = 0.0f)
        {
            Product p = new Product()
            {
                ProductCode  = productCode,
                Description  = description,
                SupplierCode = supplierCode,
                Price        = price,
                Height       = height,
                Width        = width,
                Depth        = depth,
                Weight       = weight
            };
            Session.Save(p);

            return p;
        }

        private Customer InsertCustomer(string name, string addressLine1 = "", string addressLine2 = null, string city = "",
                                        string postcode = null)
        {
            Customer c = new Customer()
            {
                Name         = name,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City         = city,
                PostCode     = postcode
            };
            Session.Save(c);

            return c;
        }

        private Order InsertOrder(Customer customer, OrderLine[] orderLines)
        {
            Order o = new Order()
            {
                Customer = customer
            };

            foreach (OrderLine line in orderLines)
            {
                line.Order = o;
                o.OrderLines.Add(line);
            }
            Session.Save(o);

            return o;
        }

        private OrderLine CreateOrderLine(Product product, int quantity = 1)
        {
            return new OrderLine()
            {
                Product  = product,
                Quantity = quantity
            };
        }


        private ISession Session
        {
            get
            {
                return NHibernateSessionManager.CurrentSession;
            }
        }
    }
}
