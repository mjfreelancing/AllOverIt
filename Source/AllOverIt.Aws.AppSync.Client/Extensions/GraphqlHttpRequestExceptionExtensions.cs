using AllOverIt.Aws.AppSync.Client.Exceptions;
using System.Linq;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    public static class GraphqlHttpRequestExceptionExtensions
    {
        private static readonly string ExecutionTimeoutErrorType = "ExecutionTimeout";

        public static bool HasExecutionTimeoutError(this GraphqlHttpRequestException exception)
        {
            return exception.Errors.Any(error => error.ErrorType == ExecutionTimeoutErrorType);
        }
    }
}