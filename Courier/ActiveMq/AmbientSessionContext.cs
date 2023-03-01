using System.Runtime.Remoting.Messaging;

namespace Courier.ActiveMq
{
    static class AmbientSessionContext
    {
        const string Key = @"{0D082F10-0321-4605-8264-70D610EF0A30}";

        public static SessionContext Current
        {
            get { return CallContext.LogicalGetData(Key) as SessionContext; }
            set { CallContext.LogicalSetData(Key, value); }
        }
    }
}
