using AllOverIt.Assertion;
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
    public sealed class CountdownTimer : ReactiveObject, ICountdownTimer
    {
        private readonly Subject<bool> _countdownCompletedSubject = new();      // publishes true if completed, false if cancelled
        private ReactiveCommand<int, Unit> _startCommand;
        private IDisposable _startDisposable;
        private IDisposable _intervalDisposable;

        public int TotalMilliseconds { get; private set; }

        [Reactive]
        public bool IsRunning { get; private set; }

        [Reactive]
        public int RemainingMilliseconds { get; private set; }

        // notifies when the current countdown completes (true) or is cancelled (false)
        public IObservable<bool> WhenCompleted() => _countdownCompletedSubject;

        // this method exists so the same countdown timer can be associate with an observable even after changing the parameters
        public void Configure(int totalMilliseconds, int updateIntervalMilliseconds, IScheduler scheduler = null, CancellationToken cancellationToken = default)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("The countdown timer period cannot be modified while executing.");
            }

            TotalMilliseconds = totalMilliseconds;

            var canStart = this.WhenAnyValue(vm => vm.IsRunning, value => !value);

            _startCommand = ReactiveCommand.Create<int, Unit>(skipMilliseconds =>
            {
                IsRunning = true;

                var remaining = TimeSpan.FromMilliseconds(totalMilliseconds - skipMilliseconds);		// it's ok if this is <= 0
                var startTime = DateTime.Now;

                var intervalObservable = Observable
                    .Interval(TimeSpan.FromMilliseconds(updateIntervalMilliseconds))
                    .Select(_ =>
                    {
                        var elapsed = DateTime.Now.Subtract(startTime);
                        return (int) remaining.Subtract(elapsed).TotalMilliseconds;
                    })
                    .TakeWhile(milliseconds => !cancellationToken.IsCancellationRequested && milliseconds > 0.0d);

                if (scheduler != null)
                {
                    intervalObservable = intervalObservable.ObserveOn(scheduler);
                }

                _intervalDisposable = intervalObservable
                    .Subscribe(
                        onNext: milliseconds => RemainingMilliseconds = milliseconds,
                        onCompleted: () =>
                        {
                            RemainingMilliseconds = 0;
                            IsRunning = false;
                            _countdownCompletedSubject.OnNext(!cancellationToken.IsCancellationRequested);
                        });

                return Unit.Default;
            }, canStart);
        }

        public void Start(int skipMilliseconds)
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

        public void Stop()
        {
            _intervalDisposable?.Dispose();
            _intervalDisposable = null;

            _startDisposable?.Dispose();
            _startDisposable = null;
        }

        public IObservable<int> ObserveRemainingSeconds()
        {
            return this
                .ObservableForProperty(vm => vm.RemainingMilliseconds)
                .Select(item => item.Value / 1000)
                .DistinctUntilChanged();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}