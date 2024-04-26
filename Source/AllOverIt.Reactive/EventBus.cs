using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AllOverIt.Reactive
{
    // Note: Not sealed to cater for scenarios where differently scoped event buses are required.

    /// <summary>Implements a subscribable event aggregator / bus that consumers can subscribe to for notification of various event types.</summary>
    public class EventBus : IEventBus
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
            return _subject.OfType<TEvent>();
        }

        /// <summary>Disposes of the internal resources.</summary>
        /// <param name="disposing">Indicates if the internal resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _subject is not null)
            {
                _subject.Dispose();
                _subject = null;
            }
        }

        /// <summary>Disposes of the observable sequence used for notifying observers of various events.</summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
