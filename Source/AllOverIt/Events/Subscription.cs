using AllOverIt.Helpers;
using System;

namespace AllOverIt.Events
{
    public class Subscription : ISubscription
    {
        private readonly Delegate _handler;

        public Subscription(Delegate handler)
        {
            _handler = handler.WhenNotNull(nameof(handler));
        }

        public Action<TMessage> GetHandler<TMessage>()
        {
            return (Action<TMessage>)_handler;
        }

        public void Handle<TMessage>(TMessage message)
        {
            GetHandler<TMessage>().Invoke(message);
        }
    }
}
