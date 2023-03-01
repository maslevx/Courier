using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier.Bus
{
    public class WireMessage
    {
        public MessageProperties Properties { get; }
        public string Body { get; }

        public WireMessage(MessageProperties properties, string body)
        {
            Properties = properties;
            Body = body;
        }
    }
}
