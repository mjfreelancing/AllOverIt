using AllOverIt.Assertion;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AllOverIt.ReactiveUI
{
    /// <summary>Provides an observable countdown timer.</summary>
    public sealed class CountdownTimer : ReactiveObject, ICountdownTimer
    {
        private bool _disposed;
        private readonly Subject<bool> _countdownCompletedSubject = new();      // publishes true if completed, false if cancelled
        private readonly IScheduler? _timerScheduler;
        private ReactiveCommand<int, Unit>? _startCommand;
        private IDisposable? _startDisposable;
        private IDisposable? _intervalDisposable;
        private bool _isRunning;
        private double _remainingMilliseconds;
        private TimeSpan _remainingTimeSpan;

        /// <inheritdoc />
        public double TotalMilliseconds { get; private set; }

        /// <inheritdoc />
        public TimeSpan TotalTimeSpan => TimeSpan.FromMilliseconds(TotalMilliseconds);

        /// <inheritdoc />
        public bool IsRunning
        {
            get => _isRunning;
            private set => this.RaiseAndSetIfChanged(ref _isRunning, value);
        }

        /// <inheritdoc />
        public double RemainingMilliseconds
        {
            get => _remainingMilliseconds;
            private set => this.RaiseAndSetIfChanged(ref _remainingMilliseconds, value);
        }

        /// <inheritdoc />
        public TimeSpan RemainingTimeSpan
        {
            get => _remainingTimeSpan;
            private set => this.RaiseAndSetIfChanged(ref _remainingTimeSpan, value);
        }

        /// <inheritdoc />
        public IObservable<bool> WhenCompleted() => _countdownCompletedSubject;

        // Only used for tests
        internal CountdownTimer(IScheduler timerScheduler)
        {
            _timerScheduler = timerScheduler;
        }

        /// <summary>Default constructor.</summary>
        public CountdownTimer()
        {
        }

        /// <inheritdoc />
        public void Configure(double totalMilliseconds, double updateIntervalMilliseconds, IScheduler? scheduler = null, CancellationToken cancellationToken = default)
        {
            Configure(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(updateIntervalMilliseconds), scheduler, cancellationToken);
        }

        /// <inheritdoc />
        public void Configure(TimeSpan totalTimeSpan, TimeSpan updateInterval, IScheduler? observeOnScheduler = null, CancellationToken cancellationToken = default)
        {
            Throw<InvalidOperationException>.When(IsRunning, "The countdown timer period cannot be modified while executing.");

            TotalMilliseconds = totalTimeSpan.TotalMilliseconds;

            var canStart = this.WhenAnyValue(vm => vm.IsRunning, value => !value);

            _startCommand = ReactiveCommand.Create<int, Unit>(skipMilliseconds =>
            {
                IsRunning = true;

                var remaining = totalTimeSpan - TimeSpan.FromMilliseconds(skipMilliseconds);		// it's ok if this is <= 0

                RemainingMilliseconds = remaining.TotalMilliseconds;
                RemainingTimeSpan = remaining;

                var scheduledInterval = _timerScheduler is not null
                    ? Observable.Interval(updateInterval, _timerScheduler).TimeInterval(_timerScheduler)
                    : Observable.Interval(updateInterval).TimeInterval();

                var intervalObservable = scheduledInterval
                    .Select(timeInterval =>
                    {
                        remaining = remaining.Subtract(timeInterval.Interval);

                        return remaining;
                    })
                    .TakeWhile(remainingTime => !cancellationToken.IsCancellationRequested && remainingTime > TimeSpan.Zero);

                if (observeOnScheduler is not null)
                {
                    intervalObservable = intervalObservable.ObserveOn(observeOnScheduler);
                }

                _intervalDisposable = intervalObservable
                    .Subscribe(
                        onNext: timeSpan =>
                        {
                            RemainingMilliseconds = timeSpan.TotalMilliseconds;
                            RemainingTimeSpan = timeSpan;
                        },
                        onCompleted: () =>
                        {
                            RemainingMilliseconds = 0;
                            RemainingTimeSpan = TimeSpan.Zero;
                            IsRunning = false;
                            _countdownCompletedSubject.OnNext(!cancellationToken.IsCancellationRequested);
                        });

                return Unit.Default;
            }, canStart);
        }

        /// <inheritdoc />
        public void Start(int skipMilliseconds = 0)
        {
#pragma warning disable CA1513 // Use ObjectDisposedException throw helper
            if (_disposed)
            {
                // Using Throw<ObjectDisposedException>() causes an AmbiguousMatchException when choosing a constructor
                throw new ObjectDisposedException("The countdown timer is already disposed.", innerException: null);
            }
#pragma warning restore CA1513 // Use ObjectDisposedException throw helper

            Throw<InvalidOperationException>.WhenNull(_startCommand, $"The {nameof(Configure)}() method must be called first.");

            Throw<InvalidOperationException>.When(IsRunning, "The countdown timer is already executing.");

            ResetDisposables();

            _startDisposable = _startCommand.Execute(skipMilliseconds).Subscribe();
        }

        /// <inheritdoc />
        public void Start(TimeSpan skipTimeSpan)
        {
            Start((int) skipTimeSpan.TotalMilliseconds);
        }

        /// <inheritdoc />
        public void Stop()
        {
            ResetDisposables();

            IsRunning = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();

                _disposed = true;
            }
        }

        private void ResetDisposables()
        {
            _intervalDisposable?.Dispose();
            _intervalDisposable = null;

            _startDisposable?.Dispose();
            _startDisposable = null;
        }
    }
}