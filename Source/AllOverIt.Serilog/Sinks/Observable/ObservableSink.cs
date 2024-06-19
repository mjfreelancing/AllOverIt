using AllOverIt.Assertion;
using Serilog.Core;
using Serilog.Events;

namespace AllOverIt.Serilog.Sinks.Observable
{
    internal sealed class ObservableSink : IObservable<LogEvent>, ILogEventSink, IDisposable
    {
        private bool _disposed;
        private readonly object _syncRoot = new();
        private List<IObserver<LogEvent>> _observers = [];

        private sealed class DisposableSubscriber : IDisposable
        {
            private readonly ObservableSink _sink;
            private readonly IObserver<LogEvent> _observer;

            public DisposableSubscriber(ObservableSink sink, IObserver<LogEvent> observer)
            {
                _sink = sink.WhenNotNull(nameof(sink));
                _observer = observer.WhenNotNull(nameof(observer));
            }

            public void Dispose()
            {
                _sink.Unsubscribe(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<LogEvent> observer)
        {
            _ = observer.WhenNotNull(nameof(observer));

            lock (_syncRoot)
            {
                //var old = _observers;
                //var newObservers = _observers.Concat([observer]).ToList();

                //while (old != Interlocked.Exchange(ref _observers, newObservers))
                //{
                //    old = _observers;
                //    newObservers = [.. _observers, observer];
                //}

                List<IObserver<LogEvent>> currentObservers;
                List<IObserver<LogEvent>> newObservers;
                SpinWait spinner = default;

                do
                {
                    spinner.SpinOnce();

                    currentObservers = _observers;
                    newObservers = [.. _observers, observer];
                } while (Interlocked.CompareExchange(ref _observers, newObservers, currentObservers) != currentObservers);
            }

            return new DisposableSubscriber(this, observer);
        }

        void Unsubscribe(IObserver<LogEvent> observer)
        {
            _ = observer.WhenNotNull(nameof(observer));

            lock (_syncRoot)
            {
                //var old = _observers;
                //var newObservers = _observers.Except([observer]).ToList();

                //while (old != Interlocked.Exchange(ref _observers, newObservers))
                //{
                //    old = _observers;
                //    newObservers = _observers.Except([observer]).ToList();
                //}

                List<IObserver<LogEvent>> currentObservers;
                List<IObserver<LogEvent>> newObservers;
                SpinWait spinner = default;

                do
                {
                    spinner.SpinOnce();

                    currentObservers = _observers;
                    newObservers = _observers.Except([observer]).ToList();
                } while (Interlocked.CompareExchange(ref _observers, newObservers, currentObservers) != currentObservers);
            }
        }

        public void Emit(LogEvent logEvent)
        {
            _ = logEvent.WhenNotNull(nameof(logEvent));

            lock (_syncRoot)
            {
                List<Exception>? exceptions = null;

                // Modifications to _observers via Subscribe() / Unsubscribe are handled by replacing the collection
                foreach (var observer in _observers)
                {
                    try
                    {
                        observer.OnNext(logEvent);
                    }
                    catch (Exception exception)
                    {
                        exceptions ??= [];
                        exceptions.Add(exception);
                    }
                }

                Throw<AggregateException>.WhenNotNull("One or more logging observers raised an error.", exceptions);
            }
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_disposed)
                {
                    return;
                }

                foreach (var observer in _observers)
                {
                    observer.OnCompleted();
                }

                _disposed = true;
            }
        }
    }
}
