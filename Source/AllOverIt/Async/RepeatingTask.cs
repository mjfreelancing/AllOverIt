using AllOverIt.Assertion;

namespace AllOverIt.Async
{
    /// <summary>Provides support for creating and executing a new task repeatedly until cancelled.</summary>
    public static class RepeatingTask
    {
        /// <summary>Creates and starts a new task after an optional initial delay that repeatedly invokes an asynchronous action.</summary>
        /// <param name="action">The action to invoke each time the task repeats.</param>
        /// <param name="options">Options that control how the task is created and behaves.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task StartAsync(Func<Task> action, RepeatingTaskOptions options, CancellationToken cancellationToken)
        {
            _ = action.WhenNotNull();
            _ = options.WhenNotNull();

            Throw<ArgumentOutOfRangeException>.When(options.InitialDelay.TotalMilliseconds < 0, $"The {nameof(RepeatingTaskOptions.InitialDelay)} cannot be negative.");
            Throw<ArgumentOutOfRangeException>.When(options.RepeatDelay.TotalMilliseconds <= 0, $"The {nameof(RepeatingTaskOptions.RepeatDelay)} must be greater than zero.");

            return Task.Factory
                .StartNew(async () =>
                {
                    // ConfigureAwait() isn't strictly required here as there is no synchronization context,
                    // but it keeps all code consistent.

                    try
                    {
                        if (options.InitialDelay.TotalMilliseconds > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await Task.Delay(options.InitialDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
                        }

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            await action.Invoke();

                            cancellationToken.ThrowIfCancellationRequested();

                            await Task.Delay(options.RepeatDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // break out
                    }
                }, cancellationToken, options.CreationOptions, options.Scheduler)
                .Unwrap();
        }

        /// <summary>Creates and starts a new task after an optional initial delay that repeatedly invokes an action.</summary>
        /// <param name="action">The action to invoke each time the task repeats.</param>
        /// <param name="options">Options that control how the task is created and behaves.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task StartAsync(Action action, RepeatingTaskOptions options, CancellationToken cancellationToken)
        {
            _ = action.WhenNotNull();
            _ = options.WhenNotNull();

            Throw<ArgumentOutOfRangeException>.When(options.InitialDelay.TotalMilliseconds < 0, $"The {nameof(RepeatingTaskOptions.InitialDelay)} cannot be negative.");
            Throw<ArgumentOutOfRangeException>.When(options.RepeatDelay.TotalMilliseconds <= 0, $"The {nameof(RepeatingTaskOptions.RepeatDelay)} must be greater than zero.");

            return Task.Factory
                .StartNew(async () =>
                {
                    // ConfigureAwait() isn't strictly required here as there is no synchronization context.
                    try
                    {
                        if (options.InitialDelay.TotalMilliseconds > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await Task.Delay(options.InitialDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
                        }

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            action.Invoke();

                            cancellationToken.ThrowIfCancellationRequested();

                            await Task.Delay(options.RepeatDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // break out
                    }
                }, cancellationToken, options.CreationOptions, options.Scheduler)
                .Unwrap();
        }
    }
}
