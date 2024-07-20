using AllOverIt.Assertion;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AllOverIt.Reactive.Messaging
{
    /// <summary>A base class handler for a message that was published by an <see cref="IEventBus"/> instance.</summary>
    /// <typeparam name="TEvent">The event type this handler will respond to.</typeparam>
    public abstract class EventBusHandler<TEvent> : ObservableObject, IDisposable
        where TEvent : notnull
    {
        private bool _disposed;
        private readonly IEventBus _eventBus;
        private IDisposable? _subscription;

        private bool _isActive;

        /// <summary>Indicates if the handler is active. Handlers are disabled (<c>IsActive = false</c>) by default to
        /// ensure automatic subscription does not occur during construction via dependency injection.</summary>
        public bool IsActive
        {
            get => _isActive;
            set => RaiseAndSetIfChanged(ref _isActive, value, null,
                () =>
                {
                    if (_isActive)
                    {
                        _subscription = OnEvent().Subscribe(Handle);
                    }
                    else
                    {
                        DisposeSubscription();
                    }
                });
        }

        /// <summary>Constructor.</summary>
        /// <param name="eventBus">The event bus to receive messages (events) from.</param>
        public EventBusHandler(IEventBus eventBus)
        {
            _eventBus = eventBus.WhenNotNull();
        }

        /// <summary>Implemented in concrete classes to provide the required event handling.</summary>
        /// <param name="event">The event instance to handle.</param>
        public abstract void Handle(TEvent @event);

        /// <summary>Provides the ability to auto-unsubscribe the handler when it is disposed.</summary>
        /// <param name="disposables">The group of disposables that will be released together.</param>
        public void DisposeUsing(CompositeDisposable disposables)
        {
            disposables.Add(this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>The base class receives the event from the event bus and makes it available for subscribing to.
        /// Override this method in concrete classes to filter or transform the event before the subscription is applied.
        /// The base method must be called first and the filter or transform is to be applied to this result.</summary>
        /// <returns>The observable notifying a new event when it is received.</returns>
        protected virtual IObservable<TEvent> OnEvent()
        {
            return _eventBus.GetEvent<TEvent>();
        }

        /// <summary>Disposes of the internal resources.</summary>
        /// <param name="disposing">Indicates if the internal resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (_subscription is not null)
            {
                if (disposing)
                {
                    DisposeSubscription();
                }
            }

            _disposed = true;
        }

        private void DisposeSubscription()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}
