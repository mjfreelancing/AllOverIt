using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AllOverIt.Reactive.Messaging
{
    // Note: Not sealed to cater for scenarios where differently scoped event buses are required.

    /// <summary>Implements an event aggregator / bus that consumers can subscribe to for notification of various event types.</summary>
    public class EventBus : IEventBus
    {
        private bool _disposed;
        private readonly Subject<object> _subject = new();

        /// <inheritdoc />
        public void Publish<TEvent>() where TEvent : notnull, new()
        {
            _subject.OnNext(new TEvent());
        }

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent @event) where TEvent : notnull
        {
            _subject.OnNext(@event);
        }

        /// <inheritdoc />
        public IObservable<TEvent> GetEvent<TEvent>() where TEvent : notnull
        {
            return _subject.OfType<TEvent>();
        }

        /// <summary>Disposes of the internal resources.</summary>
        /// <param name="disposing">Indicates if the internal resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _subject.Dispose();
                _disposed = true;
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
