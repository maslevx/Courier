namespace Courier
{
    public enum ConsumeType
    {
        Receive,
        Subscribe
    }

    public class ConsumerConfiguration
    {
        public string Destination { get; set; }
        public ConsumeType ConsumeStyle { get; set; }
    }
}
