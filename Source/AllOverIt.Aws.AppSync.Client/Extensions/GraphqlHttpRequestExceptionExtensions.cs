using AllOverIt.Aws.AppSync.Client.Exceptions;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="GraphqlHttpRequestException"/>.</summary>
    public static class GraphqlHttpRequestExceptionExtensions
    {
        private const string ExecutionTimeoutErrorType = "Http:ExecutionTimeoutException";

        /// <summary>Determines if the exception contains any errors with an error type of 'ExecutionTimeout",
        /// as reported by AppSync.</summary>
        /// <param name="exception">The exception containing errors.</param>
        /// <returns><see langword="True" /> if the exception contains any errors with an error type of 'ExecutionTimeout", otherwise <see langword="False" />.</returns>
        public static bool HasExecutionTimeoutError(this GraphqlHttpRequestException exception)
        {
            return exception.Errors != null && exception.Errors.Any(error => error.ErrorType == ExecutionTimeoutErrorType);
        }
    }
}