namespace Courier.ActiveMq
{
    public class ActiveMqConnectionFactory : IConnectionFactory
    {
        public IConnection CreateConnection(string broker)
        {
            var nmsConnection = ConnectionFactory.CreateConnection(broker);
            return new ActiveMqConnection(nmsConnection);
        }
    }
}
