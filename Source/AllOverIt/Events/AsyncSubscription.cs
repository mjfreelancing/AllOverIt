using AllOverIt.Assertion;

namespace AllOverIt.Events
{
    internal sealed class AsyncSubscription : IAsyncSubscription
    {
        private readonly Delegate _handler;

        public AsyncSubscription(Delegate handler)
        {
            _handler = handler.WhenNotNull(nameof(handler));
        }

        public Func<TMessage, Task> GetHandler<TMessage>()
        {
            return (Func<TMessage, Task>) _handler;
        }

        public Task HandleAsync<TMessage>(TMessage message)
        {
            return GetHandler<TMessage>().Invoke(message);
        }
    }
}
