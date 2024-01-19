using AllOverIt.Assertion;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Task"/> types.</summary>
    public static class TaskExtensions
    {
        /// <summary>Awaits a <see cref="Task"/> and reports any faulted state via an exception handler, if provided.</summary>
        /// <param name="task">The task to await.</param>
        /// <param name="exceptionHandler">Reports a faulted task via an <see cref="ExceptionDispatchInfo"/>.
        /// This object can be used to apply a strategy against the raised exception, or re-throw the original exception without losing stack
        /// trace information.</param>
        public static void FireAndForget(this Task task, Action<ExceptionDispatchInfo> exceptionHandler)
        {
            _ = exceptionHandler.WhenNotNull(nameof(exceptionHandler));

            _ = DoFireAndForget(task, exceptionHandler);
        }

        internal static async Task DoFireAndForget(Task task, Action<ExceptionDispatchInfo> exceptionHandler)
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
                    var dispatchInfo = ExceptionDispatchInfo.Capture(exception);

                    exceptionHandler.Invoke(dispatchInfo);
                }
            }
        }
    }
}