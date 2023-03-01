using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier
{
    public interface IBus : IDisposable
    {
        Task Publish<T>(T message) where T : class;
        Task Publish<T>(T message, string queue) where T : class;
        Task Publish<T>(IMessage<T> message) where T : class;
        Task Publish<T>(IMessage<T> message, string queue) where T : class;
        Task Publish(IMessage message, string queue);

        Task Send<T>(T message) where T : class;
        Task Send<T>(T message, string queue) where T : class;
        Task Send<T>(IMessage<T> message) where T : class;
        Task Send<T>(IMessage<T> message, string queue) where T : class;
        Task Send(IMessage message, string queue);

        IDisposable Receive<T>(Func<T, Task> onMessage) where T : class;
        IDisposable Receive<T>(Func<T, Task> onMessage, string queue) where T : class;
        IDisposable Receive<T>(Func<IMessage<T>, Task> onMessage) where T : class;
        IDisposable Receive<T>(Func<IMessage<T>, Task> onMessage, string queue) where T : class;

        IDisposable Subscribe<T>(Func<T, Task> onMessage) where T : class;
        IDisposable Subscribe<T>(Func<T, Task> onMessage, string queue) where T : class;
        IDisposable Subscribe<T>(Func<IMessage<T>, Task> onMessage) where T : class;
        IDisposable Subscribe<T>(Func<IMessage<T>, Task> onMessage, string queue) where T : class;

    }
}
