using AllOverIt.Assertion;
using AllOverIt.Collections;
using AllOverIt.Patterns.ResourceInitialization;
using Serilog.Core;
using Serilog.Events;

namespace AllOverIt.Serilog.Sinks.Observable
{
    /// <summary>Provides an observable sink for log events.</summary>
    public class ObservableSink : IObservableSink, IDisposable
    {
        private bool _disposed;
        private readonly LockableList<IObserver<LogEvent>> _observers = new(false);

        /// <inheritdoc/>
        public int Count => _observers.Count;

        /// <summary>Subscribes the observer for notification of log events.</summary>
        /// <param name="observer">The observer to be notified of log events.</param>
        /// <returns>A disposable that when disposed of will unsubscribe the observer.</returns> 
        public IDisposable Subscribe(IObserver<LogEvent> observer)
        {
            _ = observer.WhenNotNull(nameof(observer));

            // Important in Unsubscribe() so we can report when a subscriber has lived longer than the sink.
            // This is here for completeness / consistency.
#pragma warning disable CA1513 // Use ObjectDisposedException throw helper
            if (_disposed)
            {
                // DO NOT remove 'innerException' - without it, the message will be treated as the object name (different constructor)
                throw new ObjectDisposedException("The observable sink has been disposed.", innerException: null);
            }
#pragma warning restore CA1513 // Use ObjectDisposedException throw helper

            _observers.Add(observer);

            var state = (this, observer);

            return new Raii<(ObservableSink Sink, IObserver<LogEvent> Observer)>(
                () => state,
                state => state.Sink.Unsubscribe(state.Observer));
        }

        /// <summary>Notifies any subscribed observers that the sink is being disposed via their
        /// <see cref="IObserver{T}.OnCompleted"/> action.
        /// All subscribers should be disposed before disposing of this observable sink. If the sink is
        /// disposed first, then disposing of the observers at a later time will cause an <see cref="ObjectDisposedException"/>
        /// to be thrown.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes the sink's resources when <paramref name="disposing"/> is <see langword="True"/>.</summary>
        /// <param name="disposing">When <see langword="True"/>, the sink will be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
            {
                return;
            }

            // The enumerator obtains a read-lock for the duration of the enumeration.
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }

            _disposed = true;
        }

        void ILogEventSink.Emit(LogEvent logEvent)
        {
            _ = logEvent.WhenNotNull(nameof(logEvent));

            // The enumerator obtains a read-lock for the duration of the enumeration.
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(logEvent);
                }
                catch (Exception exception)
                {
                    observer.OnError(exception);
                }
            }
        }

        private void Unsubscribe(IObserver<LogEvent> observer)
        {
            // Detect a subscriber being disposed after the sink has been disposed
#pragma warning disable CA1513 // Use ObjectDisposedException throw helper
            if (_disposed)
            {
                // DO NOT remove 'innerException' - without it, the message will be treated as the object name (different constructor)
                throw new ObjectDisposedException("The observable sink has been disposed.", innerException: null);
            }
#pragma warning restore CA1513 // Use ObjectDisposedException throw helper

            _ = observer.WhenNotNull(nameof(observer));

            _observers.Remove(observer);
        }
    }
}
