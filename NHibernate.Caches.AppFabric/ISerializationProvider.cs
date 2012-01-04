using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.AppFabric
{
    public interface ISerializationProvider
    {
        byte[] Serialize(object value);
        object Deserialize(byte[] bytes);
    }
}
