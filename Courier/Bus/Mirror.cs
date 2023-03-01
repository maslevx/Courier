using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Courier.Bus
{
    static class Mirror
    {
        public static void WithAttribute<T>(this Type type, Action<T> action) where T : Attribute
        {
            T attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
                action(attribute);
        }
    }
}
