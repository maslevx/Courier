using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier
{
    public interface IMessage<out T> : IMessage
    {
        T Body { get; }
    }

    public interface IMessage
    {
        MessageProperties Properties { get; }
        Type MessageType { get; }

        object GetBody();
    }

    public class Message<T> : IMessage<T>
        where T : class
    {
        public MessageProperties Properties { get; private set; }
        public Type MessageType { get { return typeof(T); } }
        public T Body { get; private set; }

        public Message(MessageProperties properties, T body)
        {
            Properties = properties;
            Body = body;
        }

        object IMessage.GetBody()
        {
            return Body;
        }
    }

    public class MessageProperties
    {
        public IDictionary<string,string> Headers { get; set; }
        public string CorrelationId { get; set; }
        public string ReplyTo { get; set; }
        public string MessageId { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string Source { get; set; }
        public bool IsDurable { get; set; } = true;
        public int TimeToLive { get; set; }

        public MessageProperties Clone()
        {
            return new MessageProperties
            {
                CorrelationId = CorrelationId,
                ReplyTo = ReplyTo,
                MessageId = MessageId,
                Type = Type,
                ContentType = ContentType,
                Source = Source,
                Headers = Headers == null ? null : new Dictionary<string, string>(Headers)
            };
        }
    }
}
