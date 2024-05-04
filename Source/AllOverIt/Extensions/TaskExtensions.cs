using AllOverIt.Assertion;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Task"/> types.</summary>
    public static class TaskExtensions
    {
        /// <summary>Awaits a <see cref="Task"/> and reports any faulted state via an exception handler, if provided.</summary>
        /// <param name="task">The task to await.</param>
        /// <param name="exceptionHandler">Reports a faulted task.</param>
        public static void FireAndForget(this Task task, Action<Exception> exceptionHandler)
        {
            _ = exceptionHandler.WhenNotNull(nameof(exceptionHandler));

            _ = DoFireAndForget(task, exceptionHandler);
        }

        internal static async Task DoFireAndForget(Task task, Action<Exception> exceptionHandler)
        {
            if (!task.IsCompleted || task.IsFaulted)
            {
                try
                {
                    // No need to resume on the original SynchronizationContext
                    await task.ConfigureAwait(false);
                }
                catch (Exception exception) when (exceptionHandler is not null)
                {
                    exceptionHandler.Invoke(exception);
                }
            }
        }
    }
}