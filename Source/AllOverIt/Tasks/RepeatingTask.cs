using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tasks
{
    public static class RepeatingTask
    {
        public static Task Start(Func<Task> action, CancellationToken cancellationToken, int repeatDelay, int initialDelay = 0)
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

        public static Task Start(Action action, CancellationToken cancellationToken, int repeatDelay, int initialDelay = 0)
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
