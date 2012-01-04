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
using NHibernate.Caches.AppFabric.Tests.Common;
using NHibernate.Caches.AppFabric.Tests.Functional.Entities;
using NUnit.Framework;

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
