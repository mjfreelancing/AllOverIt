using System.Reactive.Concurrency;

namespace AllOverIt.ReactiveUI
{
    /// <summary>Represents an observable countdown timer.</summary>
    public interface ICountdownTimer : IDisposable
    {
        /// <summary>Gets the total countdown period, in milliseconds.</summary>
        double TotalMilliseconds { get; }

        /// <summary>Gets the total countdown period, as a <see cref="TimeSpan"/>.</summary>
        TimeSpan TotalTimeSpan { get; }

        /// <summary>Indicates if the timer is currently counting down. This property is observable.</summary>
        bool IsRunning { get; }

        /// <summary>Gets the reminaing countdown period, in milliseconds. This property is observable.</summary>
        double RemainingMilliseconds { get; }

        /// <summary>Gets the remaining countdown period, as a <see cref="TimeSpan"/>. This property is observable.</summary>
        TimeSpan RemainingTimeSpan { get; }

        /// <summary>Configures the countdown timer total period and update interval.</summary>
        /// <param name="totalMilliseconds">The total number of milliseconds to count down from.</param>
        /// <param name="updateIntervalMilliseconds">The update interval, in milliseconds.</param>
        /// <param name="scheduler">The scheduler to notify updates.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the timer when it is running.</param>
        void Configure(double totalMilliseconds, double updateIntervalMilliseconds, IScheduler scheduler = null, CancellationToken cancellationToken = default);

        /// <summary>Configures the countdown timer total period and update interval.</summary>
        /// <param name="totalTimeSpan">The total <see cref="TimeSpan"/> to count down from.</param>
        /// <param name="updateInterval">The update interval.</param>
        /// <param name="scheduler">The scheduler to notify updates.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the timer when it is running.</param>
        void Configure(TimeSpan totalTimeSpan, TimeSpan updateInterval, IScheduler scheduler = null, CancellationToken cancellationToken = default);

        /// <summary>Starts the countdown timer.</summary>
        /// <param name="skipMilliseconds">The number of milliseconds to skip (subtract from the remaining period).</param>
        void Start(int skipMilliseconds = 0);

        /// <summary>Starts the countdown timer.</summary>
        /// <param name="skipTimeSpan">The <see cref="TimeSpan"/> to skip (subtract from the remaining period).</param>
        void Start(TimeSpan skipTimeSpan);

        /// <summary>Stops the timer.</summary>
        void Stop();

        /// <summary>Notifies when the current countdown completes (true) or is cancelled (false).</summary>
        IObservable<bool> WhenCompleted();
    }
}