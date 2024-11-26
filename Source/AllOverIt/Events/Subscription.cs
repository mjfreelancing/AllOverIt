using AllOverIt.Assertion;

namespace AllOverIt.Events
{
    internal sealed class Subscription : ISubscription
    {
        private readonly Delegate _handler;

        public Subscription(Delegate handler)
        {
            _handler = handler.WhenNotNull();
        }

        public Action<TMessage> GetHandler<TMessage>()
        {
            return (Action<TMessage>) _handler;
        }

        public void Handle<TMessage>(TMessage message)
        {
            GetHandler<TMessage>().Invoke(message);
        }
    }
}
