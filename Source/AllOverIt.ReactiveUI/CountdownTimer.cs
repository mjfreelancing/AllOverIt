using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace AllOverIt.ReactiveUI
{
    /// <summary>Provides an observable countdown timer.</summary>
    public sealed class CountdownTimer : ReactiveObject, ICountdownTimer
    {
        private readonly Subject<bool> _countdownCompletedSubject = new();      // publishes true if completed, false if cancelled
        private ReactiveCommand<int, Unit> _startCommand;
        private IDisposable _startDisposable;
        private IDisposable _intervalDisposable;

        /// <inheritdoc />
        public double TotalMilliseconds { get; private set; }

        /// <inheritdoc />
        public TimeSpan TotalTimeSpan => TimeSpan.FromMilliseconds(TotalMilliseconds);

        /// <inheritdoc />
        [Reactive]
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        [Reactive]
        public double RemainingMilliseconds { get; private set; }

        /// <inheritdoc />
        [Reactive]
        public TimeSpan RemainingTimeSpan { get; private set; }

        /// <inheritdoc />
        public IObservable<bool> WhenCompleted() => _countdownCompletedSubject;

        /// <inheritdoc />
        public void Configure(double totalMilliseconds, double updateIntervalMilliseconds, IScheduler scheduler = null, CancellationToken cancellationToken = default)
        {
            Configure(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(updateIntervalMilliseconds), scheduler, cancellationToken);
        }

        /// <inheritdoc />
        public void Configure(TimeSpan totalTimeSpan, TimeSpan updateInterval, IScheduler scheduler = null, CancellationToken cancellationToken = default)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("The countdown timer period cannot be modified while executing.");
            }

            TotalMilliseconds = (int)totalTimeSpan.TotalMilliseconds;

            var canStart = this.WhenAnyValue(vm => vm.IsRunning, value => !value);

            _startCommand = ReactiveCommand.Create<int, Unit>(skipMilliseconds =>
            {
                IsRunning = true;

                var remaining = totalTimeSpan - TimeSpan.FromMilliseconds(skipMilliseconds);		// it's ok if this is <= 0
                var startTime = DateTime.Now;

                var intervalObservable = Observable
                    .Interval(updateInterval)
                    .Select(_ =>
                    {
                        var elapsed = DateTime.Now.Subtract(startTime);
                        return remaining.Subtract(elapsed);
                    })
                    .TakeWhile(remainingTime => !cancellationToken.IsCancellationRequested && remainingTime > TimeSpan.Zero);

                if (scheduler != null)
                {
                    intervalObservable = intervalObservable.ObserveOn(scheduler);
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
            if (IsRunning)
            {
                throw new InvalidOperationException("The countdown timer is already executing.");
            }

            if (_startCommand == null)
            {
                throw new InvalidOperationException($"The {nameof(Configure)}() method must be called first.");
            }

            Stop();

            _startDisposable = _startCommand.Execute(skipMilliseconds).Subscribe();
        }

        /// <inheritdoc />
        public void Start(TimeSpan skipTimeSpan)
        {
            Start((int)skipTimeSpan.TotalMilliseconds);
        }

        /// <inheritdoc />
        public void Stop()
        {
            _intervalDisposable?.Dispose();
            _intervalDisposable = null;

            _startDisposable?.Dispose();
            _startDisposable = null;

            IsRunning = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
        }
    }
}