using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier
{
    using Bus;

    public interface IConsumer
    {
        IDisposable StartConsuming(Func<WireMessage, Task> onMessage);
    }
}
