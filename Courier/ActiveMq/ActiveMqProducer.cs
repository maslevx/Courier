using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.NMS;
using Courier.Bus;

namespace Courier.ActiveMq
{
    using IConnection = Apache.NMS.IConnection;

    public class ActiveMqProducer : IProducer
    {
        private SessionContext Context { get; }

        public ActiveMqProducer(IConnection connection)
        {
            var session = connection.CreateSession();
            var sender = session.CreateProducer();

            Context = new SessionContext(session, null, sender);
        }

        public Task Publish(string destination, WireMessage message)
        {
            var context = AmbientSessionContext.Current ?? Context;

            var target = context.Session.GetDestination(destination, DestinationType.Topic);
            var nmsMessage = context.Session.CreateTextMessage();

            FillMessage(nmsMessage, message);
            context.Sender.Send(target, nmsMessage);

            return Task.CompletedTask;
        }

        public Task Send(string destination, WireMessage message)
        {
            var context = AmbientSessionContext.Current ?? Context;

            var target = context.Session.GetDestination(destination);
            var nmsMessage = context.Session.CreateTextMessage();

            FillMessage(nmsMessage, message);
            context.Sender.Send(target, nmsMessage);

            return Task.CompletedTask;
        }

        void FillMessage(ITextMessage target, WireMessage source)
        {
            target.Text = source.Body;

            var properties = source.Properties;
            target.NMSCorrelationID = properties.CorrelationId;
            target.NMSMessageId = properties.MessageId;
            target.NMSType = properties.Type;

            target.NMSDeliveryMode = properties.IsDurable ? MsgDeliveryMode.Persistent : MsgDeliveryMode.NonPersistent;
            target.NMSTimeToLive = TimeSpan.FromSeconds(properties.TimeToLive);

            if (!String.IsNullOrWhiteSpace(properties.ReplyTo))
                target.NMSReplyTo = (AmbientSessionContext.Current ?? Context).Session.GetDestination(properties.ReplyTo);
            if (!String.IsNullOrWhiteSpace(properties.ContentType))
                target.Properties.SetString(nameof(MessageProperties.ContentType), properties.ContentType);
            if (!String.IsNullOrWhiteSpace(properties.Source))
                target.Properties.SetString(nameof(MessageProperties.Source), properties.Source);

            if(properties.Headers != null)
                foreach (var header in properties.Headers)
                    target.Properties.SetString(header.Key, header.Value);
        }

        public void Dispose()
        {
            Context.Session.Close();
        }
    }
}
