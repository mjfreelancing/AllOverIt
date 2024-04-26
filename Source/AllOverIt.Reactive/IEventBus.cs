namespace AllOverIt.Reactive
{
    /// <summary>Describes a subscribable event aggregator / message bus.</summary>
    public interface IEventBus : IDisposable
    {
        /// <summary>Publishes a default constructed event.</summary>
        /// <typeparam name="TEvent"></typeparam>
        void Publish<TEvent>() where TEvent : new();

        /// <summary>Publishes a provided event instance.</summary>
        /// <typeparam name="TEvent">The event type to be published.</typeparam>
        /// <param name="event">The event instance to be published.</param>
        void Publish<TEvent>(TEvent @event);

        /// <summary>Filters the event bus for notification of an event type.</summary>
        /// <typeparam name="TEvent">The event type to filter.</typeparam>
        /// <returns>An observable that will be notified when the specified event type is received.</returns>
        IObservable<TEvent> GetEvent<TEvent>();
    }
}
