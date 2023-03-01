using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Courier.Bus;
using Serilog;

namespace Courier.ActiveMq
{
    using IConnection = Apache.NMS.IConnection;
    using IMessage = Apache.NMS.IMessage;

    public class ActiveMqConsumer : IConsumer
    {
        private SessionContext Context { get; }
        private IDestination Target { get; }
        private IConnection Connection { get; }

        public ActiveMqConsumer(IConnection connection, ConsumerConfiguration config)
        {
            ISession session = connection.CreateSession(AcknowledgementMode.Transactional);

            IDestination destination = session.GetDestination(config.Destination,
                config.ConsumeStyle == ConsumeType.Subscribe ? DestinationType.Topic : DestinationType.Queue);
            IMessageConsumer consumer = session.CreateConsumer(destination);
            IMessageProducer sender = session.CreateProducer();

            Context = new SessionContext(session, consumer, sender);
            Target = destination;
            Connection = connection;
        }

        private CancellationTokenSource cancelToken;
        private Task consumeTask;

        public IDisposable StartConsuming(Func<WireMessage, Task> onMessage)
        {
            if (cancelToken != null) throw new InvalidOperationException("Already consuming");

            cancelToken = new CancellationTokenSource();
            consumeTask = Task.Run(() => ConsumeMessages(onMessage), cancelToken.Token);

            return new Janitor(() =>
            {
                cancelToken.Cancel();
                consumeTask.Wait();
                Context.Session.Close();
            });
        }

        private async Task ConsumeMessages(Func<WireMessage, Task> onMessage)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(2);

            while(cancelToken.IsCancellationRequested == false)
            {
                try
                {
                    IMessage nmsMessage = Context.Consumer.Receive(timeout);
                    if(nmsMessage != null)
                    {
                        AmbientSessionContext.Current = Context;

                        WireMessage message = Convert(nmsMessage);
                        await onMessage(message);

                        Context.Session.Commit();
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e, "Error consuming message from {Target}", Target);
                    Context.Session.Rollback();
                }
                finally
                {
                    AmbientSessionContext.Current = null;
                }
            }
        }

        private WireMessage Convert(IMessage message)
        {
            ITextMessage textmessage = message as ITextMessage;
            if (textmessage == null) throw new Exception("Can't convert message: " + message.NMSMessageId);

            MessageProperties properties = GetProperties(textmessage);
            string body = textmessage.Text;

            return new WireMessage(properties, body);
        }

        private MessageProperties GetProperties(ITextMessage message)
        {
            var properties = new MessageProperties
            {
                CorrelationId = message.NMSCorrelationID,
                MessageId = message.NMSMessageId,
                Type = message.NMSType,
                IsDurable = message.NMSDeliveryMode == MsgDeliveryMode.Persistent,
                TimeToLive = (int) message.NMSTimeToLive.TotalSeconds,
                Headers = new Dictionary<string, string>()
            };

            if (message.NMSReplyTo != null)
                properties.ReplyTo = message.NMSReplyTo.ToString();

            foreach(string key in message.Properties.Keys)
                switch(key)
                {
                    case nameof(MessageProperties.ContentType):
                        properties.ContentType = (string) message.Properties[key];
                        break;
                    case nameof(MessageProperties.Source):
                        properties.Source = (string) message.Properties[key];
                        break;
                    default:
                        properties.Headers.Add(key, message.Properties[key]?.ToString() ?? String.Empty);
                        break;
                }

            return properties;
        }
    }
}
