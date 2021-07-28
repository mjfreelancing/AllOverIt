﻿using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly IDictionary<Type, IList<ISubscription>> _subscriptions = new Dictionary<Type, IList<ISubscription>>();
        private readonly IDictionary<Type, IList<IAsyncSubscription>> _asyncSubscriptions = new Dictionary<Type, IList<IAsyncSubscription>>();

        public void Publish<TMessage>(TMessage message)
        {
            if (_asyncSubscriptions.TryGetValue(typeof(TMessage), out _))
            {
                throw new InvalidOperationException("Cannot publish message when async subscriptions exist");
            }

            PublishToSubscriptions(message);
        }

        public Task PublishAsync<TMessage>(TMessage message)
        {
            var publishTask = PublishToAsyncSubscriptions(message);
            PublishToSubscriptions(message);

            return publishTask;
        }

        public void Subscribe<TMessage>(Action<TMessage> handler, bool weakSubscription = true)
        {
            var subscription = weakSubscription
              ? (ISubscription)new WeakSubscription(handler)
              : new Subscription(handler);

            Subscribe<TMessage>(subscription);
        }

        public void Subscribe<TMessage>(Func<TMessage, Task> handler, bool weakSubscription = true)
        {
            var subscription = weakSubscription
              ? (IAsyncSubscription)new AsyncWeakSubscription(handler)
              : new AsyncSubscription(handler);

            Subscribe<TMessage>(subscription);
        }

        public void Unsubscribe<TMessage>(Action<TMessage> handler)
        {
            if (_subscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                foreach (var subscription in subscriptions)
                {
                    var action = subscription.GetHandler<TMessage>();

                    if (action == handler)
                    {
                        subscriptions.Remove(subscription);
                        return;
                    }
                }
            }
        }

        public void Unsubscribe<TMessage>(Func<TMessage, Task> handler)
        {
            if (_asyncSubscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                foreach (var subscription in subscriptions)
                {
                    var action = subscription.GetHandler<TMessage>();

                    if (action == handler)
                    {
                        subscriptions.Remove(subscription);
                        return;
                    }
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
                subscriptions = new List<ISubscription> { subscription };
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
                subscriptions = new List<IAsyncSubscription> { subscription };
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
