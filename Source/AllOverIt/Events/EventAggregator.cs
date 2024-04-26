using AllOverIt.Extensions;

namespace AllOverIt.Events
{
    /// <inheritdoc cref="IEventAggregator" />
    public sealed class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, IList<ISubscription>> _subscriptions = [];
        private readonly Dictionary<Type, IList<IAsyncSubscription>> _asyncSubscriptions = [];

        /// <inheritdoc />
        /// <remarks>Cannot be used when there are registered async subscriptions for the provided message type. Use
        /// <see cref="PublishAsync{TMessage}"/> instead.</remarks>
        public void Publish<TMessage>(TMessage message)
        {
            if (_asyncSubscriptions.TryGetValue(typeof(TMessage), out _))
            {
                throw new InvalidOperationException("Cannot publish message when async subscriptions exist");
            }

            PublishToSubscriptions(message);
        }

        /// <inheritdoc />
        public Task PublishAsync<TMessage>(TMessage message)
        {
            var publishTask = PublishToAsyncSubscriptions(message);
            PublishToSubscriptions(message);

            return publishTask;
        }

        /// <inheritdoc />
        public void Subscribe<TMessage>(Action<TMessage> handler, bool weakSubscription = true)
        {
            var subscription = weakSubscription
              ? (ISubscription) new WeakSubscription(handler)
              : new Subscription(handler);

            Subscribe<TMessage>(subscription);
        }

        /// <inheritdoc />
        public void Subscribe<TMessage>(Func<TMessage, Task> handler, bool weakSubscription = true)
        {
            var subscription = weakSubscription
              ? (IAsyncSubscription) new AsyncWeakSubscription(handler)
              : new AsyncSubscription(handler);

            Subscribe<TMessage>(subscription);
        }

        /// <inheritdoc />
        public void Unsubscribe<TMessage>(Action<TMessage> handler)
        {
            if (_subscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                // We're not checking for duplicate handler subscriptions so make sure any duplicates are unsubscribed
                var subscriptionsToRemove = subscriptions
                    .Where(subscription => subscription.GetHandler<TMessage>() == handler)
                    .SelectAsReadOnlyCollection(subscription => subscription);

                foreach (var subscription in subscriptionsToRemove)
                {
                    subscriptions.Remove(subscription);
                }
            }
        }

        /// <inheritdoc />
        public void Unsubscribe<TMessage>(Func<TMessage, Task> handler)
        {
            if (_asyncSubscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                // We're not checking for duplicate handler subscriptions so make sure any duplicates are unsubscribed
                var subscriptionsToRemove = subscriptions
                    .Where(subscription => subscription.GetHandler<TMessage>() == handler)
                    .SelectAsReadOnlyCollection(subscription => subscription);

                foreach (var subscription in subscriptionsToRemove)
                {
                    subscriptions.Remove(subscription);
                }
            }
        }

        private void Subscribe<TMessage>(ISubscription subscription)
        {
            if (_subscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                subscriptions.Add(subscription);
            }
            else
            {
                subscriptions = [subscription];
                _subscriptions.Add(typeof(TMessage), subscriptions);
            }
        }

        private void Subscribe<TMessage>(IAsyncSubscription subscription)
        {
            if (_asyncSubscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                subscriptions.Add(subscription);
            }
            else
            {
                subscriptions = [subscription];
                _asyncSubscriptions.Add(typeof(TMessage), subscriptions);
            }
        }

        private void PublishToSubscriptions<TMessage>(TMessage message)
        {
            if (_subscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                foreach (var subscription in subscriptions)
                {
                    subscription.Handle(message);
                }
            }
        }

        private Task PublishToAsyncSubscriptions<TMessage>(TMessage message)
        {
            if (!_asyncSubscriptions.TryGetValue(typeof(TMessage), out var asyncSubscriptions) ||
                !asyncSubscriptions.Any())
            {
                return Task.CompletedTask;
            }

            var tasks = asyncSubscriptions
                .Select(subscription => subscription.HandleAsync(message))
                .AsReadOnlyCollection();

            return Task.WhenAll(tasks);
        }
    }
}
