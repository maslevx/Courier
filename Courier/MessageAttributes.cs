using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueueNameAttribute : Attribute
    {
        public string Name { get; }

        public QueueNameAttribute(string name)
        {
            throw new NotImplementedException("Not doing for now, specify destination in IBus call");
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DeliveryAttribute : Attribute
    {
        public bool IsDurable { get; set; } = true;
        public int TimeToLive { get; set; }
    }
}
