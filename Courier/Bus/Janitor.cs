using System;

namespace Courier.Bus
{
    class Janitor : IDisposable
    {
        private readonly Action cleanup;

        public Janitor(Action dispose)
        {
            cleanup = dispose;
        }

        private bool disposed = false;
        void IDisposable.Dispose()
        {
            if(!disposed)
            {
                cleanup();
                disposed = true;
            }
        }
    }
}
