namespace SubscribedEvents
{
    public sealed class OddEvent
    {
        public int Value { get; }

        public OddEvent(int value)
        {
            Value = value;
        }
    }
}