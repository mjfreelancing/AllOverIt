using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tasks
{
    /// <summary>Provides support for creating and executing a new task repeatedly until cancelled.</summary>
    public static class RepeatingTask
    {
        /// <summary>Creates and starts a new task that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <returns>The started task.</returns>
        public static Task Start(Func<Task> action, int repeatDelay, CancellationToken cancellationToken, int initialDelay = 0)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (initialDelay > 0)
                    {
                        await InvokeIfNotCancelled(() => Task.Delay(initialDelay, cancellationToken), cancellationToken);
                    }

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await InvokeIfNotCancelled(action, cancellationToken);
                        await InvokeIfNotCancelled(() => Task.Delay(repeatDelay, cancellationToken), cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // break out
                }
                catch (TimeoutException)
                {
                    // break out
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        /// <summary>Creates and starts a new task that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <returns>The started task.</returns>
        public static Task Start(Action action, int repeatDelay, CancellationToken cancellationToken, int initialDelay = 0)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (initialDelay > 0)
                    {
                        await InvokeIfNotCancelled(() => Task.Delay(initialDelay, cancellationToken), cancellationToken);
                    }

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            action.Invoke();
                        }

                        await InvokeIfNotCancelled(() => Task.Delay(repeatDelay, cancellationToken), cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // break out
                }
                catch (TimeoutException)
                {
                    // break out
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        private static Task InvokeIfNotCancelled(Func<Task> action, CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested ? Task.CompletedTask : action.Invoke();
        }
    }
}
