
namespace Courier.ActiveMq
{
    using IConnection = Apache.NMS.IConnection;

    public class ActiveMqConnection : Courier.IConnection
    {
        public bool IsConnected
        {
            get { return Connection.IsStarted; }
        }
        private IConnection Connection { get; }

        public ActiveMqConnection(IConnection connection)
        {
            Connection = connection;
        }

        public IConsumer CreateConsumer(ConsumerConfiguration configuration)
        {
            return new ActiveMqConsumer(Connection, configuration);
        }

        public IProducer CreateProducer()
        {
            return new ActiveMqProducer(Connection);
        }

        private readonly object locker = new object();
        public void Initialize()
        {
            lock(locker)
            {
                if (Connection.IsStarted == false)
                    Connection.Start();
            }
        }

        #region IDisposable Support

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if(disposing)
                {
                    // todo: track & dispose producers/consumers built on this connection
                    Connection.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
