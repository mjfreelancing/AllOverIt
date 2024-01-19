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
        public static Task Start(Func<Task> action, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, 0, repeatDelay, cancellationToken);
        }

        /// <summary>Creates and starts a new task that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period
        /// restarts at the completion of each iteration.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task Start(Func<Task> action, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, 0, (int)repeatDelay.TotalMilliseconds, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task Start(Func<Task> action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, initialDelay, repeatDelay, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (TimeSpan) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task Start(Func<Task> action, TimeSpan initialDelay, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, (int) initialDelay.TotalMilliseconds, (int) repeatDelay.TotalMilliseconds, cancellationToken);
        }

        /// <summary>Creates and starts a new task that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task Start(Action action, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(repeatDelay), cancellationToken);
        }

        /// <summary>Creates and starts a new task that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task Start(Action action, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, TimeSpan.FromMilliseconds(0), repeatDelay, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task Start(Action action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, TimeSpan.FromMilliseconds(initialDelay), TimeSpan.FromMilliseconds(repeatDelay), cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (TimeSpan) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (TimeSpan) the task should repeat. This delay period restarts at the completion
        /// of each iteration.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>A task that completes when the <paramref name="cancellationToken"/> is cancelled.</returns>
        public static Task Start(Action action, TimeSpan initialDelay, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, initialDelay, repeatDelay, cancellationToken);
        }

        private static Task StartImpl(Func<Task> action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                // ConfigureAwait() isn't strictly required here as there is no synchronization context,
                // but it keeps all code consistent.

                try
                {
                    if (initialDelay > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(initialDelay, cancellationToken).ConfigureAwait(false);
                    }

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await action.Invoke();

                        cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(repeatDelay, cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // break out
                }
            }, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();
        }

        private static Task StartImpl(Action action, TimeSpan initialDelay, TimeSpan repeatDelay, CancellationToken cancellationToken)
        {
            return Task.Factory
                .StartNew(async () =>
                {
                    // ConfigureAwait() isn't strictly required here as there is no synchronization context.
                    try
                    {
                        if (initialDelay.TotalMilliseconds > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await Task.Delay(initialDelay, cancellationToken);
                        }

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            action.Invoke();

                            cancellationToken.ThrowIfCancellationRequested();

                            await Task.Delay(repeatDelay, cancellationToken);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // break out
                    }
                }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                .Unwrap();
        }
    }
}
