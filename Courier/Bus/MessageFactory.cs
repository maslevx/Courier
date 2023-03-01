using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier.Bus
{
    class MessageFactory
    {
        public static IMessage WrapMessage<T>(T message) where T : class
        {
            var properties = DefaultPropertiesFor(typeof(T));
            return new Message<T>(properties, message);
        }

        static MessageProperties DefaultPropertiesFor(Type type)
        {
            var result = new MessageProperties {Headers = new Dictionary<string, string>()};
            type.WithAttribute<DeliveryAttribute>(x =>
            {
                result.IsDurable = x.IsDurable;
                result.TimeToLive = x.TimeToLive;
            });
            result.Type = type.FullName;

            return result;
        }
    }
}
