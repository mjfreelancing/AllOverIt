using AllOverIt.Assertion;
using System;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Exception"/> types.</summary>
    public static class ExceptionExtensions
    {
        /// <summary>Walks an exception, including AggregateException, and its' inner exceptions down to a given depth.
        /// The root exception is at depth 0.</summary>
        /// <param name="exception">The exception to be walked.</param>
        /// <param name="onException">An action used to process each exception in the object graph.</param>
        /// <param name="maxRecursionDepth">The maximum depth to traverse. A value of zero will result in only the
        /// provided exception being handled. Any negative value will result in the full object graph being traversed.</param>
        public static void Walk(this Exception exception, Action<Exception> onException, int maxRecursionDepth = -1)
        {
            _ = onException.WhenNotNull(nameof(onException));

            WalkRecursive(exception, onException, 0, maxRecursionDepth);
        }

        private static void WalkRecursive(Exception exception, Action<Exception> onException, int recursionLevel, int maxRecursionLevel)
        {
            onException.Invoke(exception);

            if (exception is null || maxRecursionLevel >= 0 && recursionLevel >= maxRecursionLevel)
            {
                return;
            }

            if (exception is AggregateException aggregate && aggregate.InnerExceptions is not null)
            {
                foreach (var innerException in aggregate.InnerExceptions)
                {
                    WalkRecursive(innerException, onException, recursionLevel + 1, maxRecursionLevel);
                }

                return;
            }
            
            if (exception.InnerException is not null)
            {
                WalkRecursive(exception.InnerException, onException, recursionLevel + 1, maxRecursionLevel);
            }
        }
    }
}