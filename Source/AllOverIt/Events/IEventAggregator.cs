namespace AllOverIt.Events
{
    /// <summary>Provides a mechanism for produces to publish messages and consumers to subscribe for notification of those messages.</summary>
    public interface IEventAggregator
    {
        /// <summary>Publishes a message and delivers it to all subscribers.</summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="message">The message instance.</param>
        void Publish<TMessage>(TMessage message);

        /// <summary>Asynchronously publishes a message and delivers it to all subscribers.</summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <returns>A task that completes after all subscribers have handled the message.</returns>
        Task PublishAsync<TMessage>(TMessage message);

        /// <summary>Registers a message handler for a given message type.</summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The message handler for the registered message type.</param>
        /// <param name="weakSubscription">Indicates if the handler should be registered as a weak subscription. This is the default behaviour.</param>
        void Subscribe<TMessage>(Action<TMessage> handler, bool weakSubscription = true);

        /// <summary>Registers an asynchronous message handler for a given message type.</summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The message handler for the registered message type.</param>
        /// <param name="weakSubscription">Indicates if the handler should be registered as a weak subscription. This is the default behaviour.</param>
        void Subscribe<TMessage>(Func<TMessage, Task> handler, bool weakSubscription = true);

        /// <summary>Unsubscribes a previously registered message handler.</summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The message handler to unsubscribe.</param>
        void Unsubscribe<TMessage>(Action<TMessage> handler);

        /// <summary>Unsubscribes a previously registered asynchronous message handler.</summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The message handler to unsubscribe.</param>
        void Unsubscribe<TMessage>(Func<TMessage, Task> handler);
    }
}
