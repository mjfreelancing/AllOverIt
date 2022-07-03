using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace AllOverIt.ReactiveUI
{
    public interface ICountdownTimer : IDisposable
    {
        int TotalMilliseconds { get; }
        bool IsRunning { get; }
        int RemainingMilliseconds { get; }
        void Configure(int totalMilliseconds, int updateIntervalMilliseconds, IScheduler scheduler = null, CancellationToken cancellationToken = default);
        void Start(int skipMilliseconds);
        void Stop();
        IObservable<bool> WhenCompleted();
        IObservable<int> ObserveRemainingSeconds();
    }
}