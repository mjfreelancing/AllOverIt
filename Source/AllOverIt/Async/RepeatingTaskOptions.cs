namespace AllOverIt.Async
{
    /// <summary>Provides options that determine how <see cref="RepeatingTask"/> is constructed and behaves.</summary>
    public sealed class RepeatingTaskOptions
    {
        /// <summary>An initial delay to wait before executing the first invocation of a repeating action.</summary>
        public TimeSpan InitialDelay { get; init; } = TimeSpan.Zero;

        /// <summary>The delay period between the end of one invocation, and the start of the next.</summary>
        public TimeSpan RepeatDelay { get; init; }

        /// <summary>Provides flags that control the behavior for the creation and execution of the repeating task.</summary>
        public TaskCreationOptions CreationOptions { get; init; } = TaskCreationOptions.LongRunning;

        /// <summary>The schedular handling the low-level queuing of tasks onto threads.</summary>
        public TaskScheduler Scheduler { get; init; } = TaskScheduler.Default;

        /// <summary>Provides an abstraction for time.</summary>
        public TimeProvider TimeProvider { get; init; } = TimeProvider.System;
    }
}
