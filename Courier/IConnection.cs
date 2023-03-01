using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier
{
    public interface IConnectionFactory
    {
        IConnection CreateConnection(string uri);
    }

    public interface IConnection : IDisposable
    {
        bool IsConnection { get; }

        void Initialize();

        IConsumer CreateConsumer(ConsumerConfiguration configuration);

        IProducer CreateProducer();
    }
}
