namespace Courier
{
    using ActiveMq;
    using Bus;

    public class BusFactory
    {
        // todo: expand supportfor additional broker implementations

        public static IBus CreateBus(string broker)
        {
            return new Bus.Bus(new ActiveMqConnectionFactory(), new JsonSerializer(), broker);
        }
    }
}
