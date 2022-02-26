using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AllOverIt.Reactive
{
    /// <summary>Implements a subscribable event aggregator / bus that consumers can subscribe to for notification of various event types.</summary>
    public sealed class EventBus : IEventBus
    {
        private Subject<object> _subject = new();

        /// <inheritdoc />
        public void Publish<TEvent>() where TEvent : new()
        {
            _subject.OnNext(new TEvent());
        }

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent @event)
        {
            _subject.OnNext(@event);
        }

        /// <inheritdoc />
        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return _subject.OfType<TEvent>().AsObservable();
        }

        /// <summary>Disposes of the observable sequence used for notifying observers of various events.</summary>
        public void Dispose()
        {
            _subject?.Dispose();
            _subject = null;
        }
    }
}
