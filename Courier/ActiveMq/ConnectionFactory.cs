using System;

namespace Courier.ActiveMq
{
    using Apache.NMS;

    class ConnectionFactory
    {
        public static IConnection CreateConnection(Uri address)
        {
            IConnectionFactory factory = new NMSConnectionFactory(address);
            return factory.CreateConnection();
        }

        public static IConnection CreateConnection(string address)
        {
            return CreateConnection(new Uri(address));
        }
    }
}
