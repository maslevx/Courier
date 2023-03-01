using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace Courier.Bus
{
    public class Bus : IBus
    {
        private IConnection Connection { get; }
        private ISerializer Serializer { get; }
        private IProducer Producer { get; }

        private List<IConsumer> consumers = new List<IConsumer>();

        public Bus(IConnectionFactory connectionFactory, ISerializer serializer, string broker)
        {
            Serializer = serializer;
            Connection = connectionFactory.CreateConnection(broker);

            Connection.Initialize();
            Producer = Connection.CreateProducer();
        }

        #region " Publish "

        public async Task Publish(IMessage message, string queue)
        {
            Log.Verbose("[Begin Publish] {Destination} {@Message}", queue, message);

            var wiremessage = ConvertMessage(message);
            await Producer.Publish(queue, wiremessage);

            Log.Verbose("[End Publish] {Destination} {@Message", queue, message);
        }

        public Task Publish<T>(IMessage<T> message) where T : class
        {
            string destination = DestinationFor(typeof(T));
            return Publish(message, destination);
        }

        public Task Publish<T>(T message) where T : class
        {
            string destination = DestinationFor(typeof(T));
            return Publish(message, destination);
        }

        public Task Publish<T>(IMessage<T> message, string queue) where T : class
        {
            return Publish((IMessage) message, queue);
        }

        public Task Publish<T>(T message, string queue) where T : class
        {
            IMessage wrapped = MessageFactory.WrapMessage(message);
            return Publish(wrapped, queue);
        }


        #endregion

        #region " Send "

        public async Task Send(IMessage message, string queue)
        {
            Log.Verbose("[Begin Send] {Destination} {@Message}", queue, message);

            var wiremessage = ConvertMessage(message);
            await Producer.Send(queue, wiremessage);

            Log.Verbose("[End Send] {Destination} {@Message", queue, message);
        }

        public Task Send<T>(IMessage<T> message) where T : class
        {
            string destination = DestinationFor(typeof(T));
            return Send(message, destination);
        }

        public Task Send<T>(T message) where T : class
        {
            string destination = DestinationFor(typeof(T));
            return Send(message, destination);
        }

        public Task Send<T>(IMessage<T> message, string queue) where T : class
        {
            return Send((IMessage)message, queue);
        }

        public Task Send<T>(T message, string queue) where T : class
        {
            IMessage wrapped = MessageFactory.WrapMessage(message);
            return Send(wrapped, queue);
        }

        #endregion

        #region " Receive "

        public IDisposable Receive<T>(Func<T, Task> onMessage) where T : class
        {
            var destination = DestinationFor(typeof(T));
            return Receive(onMessage, destination);
        }

        public IDisposable Receive<T>(Func<T,Task> onMessage, string queue) where T : class
        {
            var config = new ConsumerConfiguration {ConsumeStyle = ConsumeType.Receive, Destination = queue};
            return CreateConsumer(onMessage, config);
        }

        public IDisposable Receive<T>(Func<IMessage<T>, Task> onMessage) where T : class
        {
            var destination = DestinationFor(typeof(T));
            return Receive(onMessage, destination);
        }

        public IDisposable Receive<T>(Func<IMessage<T>, Task> onMessage, string queue) where T : class
        {
            var config = new ConsumerConfiguration {ConsumeStyle = ConsumeType.Receive, Destination = queue};
            return CreateConsumer(onMessage, config);
        }

        #endregion

        #region " Subscribe "

        public IDisposable Subscribe<T>(Func<T, Task> onMessage) where T : class
        {
            var destination = DestinationFor(typeof(T));
            return Subscribe(onMessage, destination);
        }

        public IDisposable Subscribe<T>(Func<T, Task> onMessage, string queue) where T : class
        {
            var config = new ConsumerConfiguration { ConsumeStyle = ConsumeType.Subscribe, Destination = queue };
            return CreateConsumer(onMessage, config);
        }

        public IDisposable Subscribe<T>(Func<IMessage<T>, Task> onMessage) where T : class
        {
            var destination = DestinationFor(typeof(T));
            return Subscribe(onMessage, destination);
        }

        public IDisposable Subscribe<T>(Func<IMessage<T>, Task> onMessage, string queue) where T : class
        {
            var config = new ConsumerConfiguration { ConsumeStyle = ConsumeType.Subscribe, Destination = queue };
            return CreateConsumer(onMessage, config);
        }

        #endregion

        #region " Internals "

        private IDisposable CreateConsumer<T>(Func<T, Task> onMessage, ConsumerConfiguration config) where T : class
        {
            Func<IMessage<T>, Task> wrapper = (message) => onMessage(message.Body);
            return CreateConsumer(wrapper, config);
        }

        private IDisposable CreateConsumer<T>(Func<IMessage<T>, Task> onMessage, ConsumerConfiguration config) where T : class
        {
            var consumer = Connection.CreateConsumer(config);
            var handle = consumer.StartConsuming(async rawMessage =>
            {
                Log.Verbose("[Begin Consume] {Destination} {@Message}", config.Destination, rawMessage);

                var message = ConvertMessage<T>(rawMessage);
                await onMessage(message);

                Log.Verbose("[End Consume] {Destination} {@Message}", config.Destination, rawMessage);
            });

            consumers.Add(consumer);

            return new Janitor(() =>
            {
                consumers.Remove(consumer);
                handle.Dispose();
            });
        }

        private Message<T> ConvertMessage<T>(WireMessage raw) where T : class
        {
            T body = (typeof(T) == typeof(string))
                ? raw.Body as T
                : Serializer.Deserialize<T>(raw.Body);
            var properties = raw.Properties.Clone();

            return new Message<T>(properties, body);
        }

        private WireMessage ConvertMessage(IMessage message)
        {
            string body = (message.MessageType == typeof(string))
                ? (string) message.GetBody()
                : Serializer.Serialize(message.GetBody());
            MessageProperties properties = message.Properties.Clone();

            return new WireMessage(properties, body);
        }

        private string DestinationFor(Type type)
        {
            return type.FullName;
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    Producer.Dispose();
                    Connection.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
