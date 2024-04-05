using AllOverIt.Assertion;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Async
{
    /// <summary>Provides support for creating and executing a new task repeatedly until cancelled.</summary>
    public static class RepeatingTask
    {
        /// <summary>Creates and starts a new task that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat. This delay period
        /// restarts at the completion of each iteration.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Func<Task> action, int repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                RepeatDelay = TimeSpan.FromMilliseconds(repeatDelay)
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period
        /// restarts at the completion of each iteration.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Func<Task> action, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                RepeatDelay = repeatDelay
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Func<Task> action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                InitialDelay = TimeSpan.FromMilliseconds(initialDelay),
                RepeatDelay = TimeSpan.FromMilliseconds(repeatDelay)
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (TimeSpan) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Func<Task> action, TimeSpan initialDelay, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                InitialDelay = initialDelay,
                RepeatDelay = repeatDelay
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Action action, int repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                RepeatDelay = TimeSpan.FromMilliseconds(repeatDelay)
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Action action, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                RepeatDelay = repeatDelay
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Action action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                InitialDelay = TimeSpan.FromMilliseconds(initialDelay),
                RepeatDelay = TimeSpan.FromMilliseconds(repeatDelay)
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (TimeSpan) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        [Obsolete("This method will be dropped in v8. Use StartAsync() instead.")]
        public static Task Start(Action action, TimeSpan initialDelay, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                InitialDelay = initialDelay,
                RepeatDelay = repeatDelay
            };

            return StartAsync(action, options, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an optional initial delay that repeatedly invokes an asynchronous action.</summary>
        /// <param name="action">The action to invoke each time the task repeats.</param>
        /// <param name="options">Options that control how the task is created and behaves.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task StartAsync(Func<Task> action, RepeatingTaskOptions options, CancellationToken cancellationToken)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = options.WhenNotNull(nameof(options));

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

#if NET8_0_OR_GREATER
                            await Task.Delay(options.InitialDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
#else
                            await Task.Delay(options.InitialDelay, cancellationToken).ConfigureAwait(false);
#endif
                        }

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            await action.Invoke();

                            cancellationToken.ThrowIfCancellationRequested();

#if NET8_0_OR_GREATER
                            await Task.Delay(options.RepeatDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
#else
                            await Task.Delay(options.RepeatDelay, cancellationToken).ConfigureAwait(false);
#endif
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
            _ = action.WhenNotNull(nameof(action));
            _ = options.WhenNotNull(nameof(options));

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

#if NET8_0_OR_GREATER
                            await Task.Delay(options.InitialDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
#else
                            await Task.Delay(options.InitialDelay, cancellationToken).ConfigureAwait(false);
#endif
                        }

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            action.Invoke();

                            cancellationToken.ThrowIfCancellationRequested();

#if NET8_0_OR_GREATER
                            await Task.Delay(options.RepeatDelay, options.TimeProvider, cancellationToken).ConfigureAwait(false);
#else
                            await Task.Delay(options.RepeatDelay, cancellationToken).ConfigureAwait(false);
#endif
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
