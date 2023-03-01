using Apache.NMS;

namespace Courier.ActiveMq
{
    class SessionContext
    {
        public ISession Session { get; }
        public IMessageConsumer Consumer { get; }
        public IMessageProducer Sender { get; }

        public SessionContext(ISession session, IMessageConsumer consumer, IMessageProducer sender)
        {
            Session = session;
            Consumer = consumer;
            Sender = sender;
        }
    }
}
