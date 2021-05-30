using System;
using System.Threading.Tasks;

namespace AllOverIt.Events
{
    public interface IEventAggregator
    {
        // only applicable when there are no async subscriptions for the provided message type
        void Publish<TMessage>(TMessage message);

        Task PublishAsync<TMessage>(TMessage message);

        void Subscribe<TMessage>(Action<TMessage> handler, bool weakSubscription = true);

        void Subscribe<TMessage>(Func<TMessage, Task> handler, bool weakSubscription = true);

        void Unsubscribe<TMessage>(Action<TMessage> handler);

        void Unsubscribe<TMessage>(Func<TMessage, Task> handler);
    }
}
