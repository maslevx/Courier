using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier
{
    using Bus;

    public interface IProducer : IDisposable
    {
        Task Send(string destination, WireMessage message);
        Task Publish(string destination, WireMessage message);
    }
}
