#nullable enable

using AllOverIt.Assertion;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace AllOverIt.Logging.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogCall(this ILogger logger, object? caller, LogLevel logLevel = LogLevel.Information,
            [CallerMemberName] string callerName = "")
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));

            var fullName = LogCallOptions.GetCallerFullName(caller, callerName);

            logger.Log(logLevel, LogCallOptions.Instance.LogTemplateWithNoArguments, fullName);
        }

        public static void LogCall(this ILogger logger, object? caller, object arguments, LogLevel logLevel = LogLevel.Information,
            [CallerMemberName] string callerName = "")
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = arguments.WhenNotNull(nameof(arguments));
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));

            var fullName = LogCallOptions.GetCallerFullName(caller, callerName);

            logger.Log(logLevel, LogCallOptions.Instance.LogTemplateWithArguments, fullName, arguments);
        }

        public static void LogException(this ILogger logger, Exception exception, string? messageTemplate = default,
            params object?[] args)
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = exception.WhenNotNull(nameof(exception));

            if (messageTemplate is null)
            {
                logger.LogError(exception, LogCallOptions.Instance.LogExceptionTemplate, exception.Message);
            }
            else
            {
                logger.LogError(exception, messageTemplate, args);
            }
        }

        public static void LogAllExceptions(this ILogger logger, Exception exception, string? messageTemplate = default,
            params object?[] args)
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = exception.WhenNotNull(nameof(exception));

            logger.LogException(exception, messageTemplate, args);

            var innerException = exception.InnerException;

            while (innerException is not null)
            {
                logger.LogException(innerException);

                innerException = innerException.InnerException;
            }
        }
    }
}
