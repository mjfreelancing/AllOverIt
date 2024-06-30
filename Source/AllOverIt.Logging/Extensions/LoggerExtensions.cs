#nullable enable

using AllOverIt.Assertion;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace AllOverIt.Logging.Extensions
{
    /// <summary>Provides extension methods for <see cref="ILogger"/>.</summary>
    public static class LoggerExtensions
    {
        /// <summary>Logs a method call.</summary>
        /// <param name="logger">The logger used to log the method call.</param>
        /// <param name="caller">The object instance being logged. Use <see langword="null"/> for static methods.</param>
        /// <param name="logLevel">The logging level.</param>
        /// <param name="callerName">The name of the method being logged. When not used, the called method's name will be used.</param>
        /// <remarks>See <see cref="LogCallOptions.UseCallPrefix(string)"/> and <see cref="LogCallOptions.UseMethodNameProperty(string)"/>
        /// to change the format of the message template.</remarks>
        public static void LogCall(this ILogger logger, object? caller, LogLevel logLevel = LogLevel.Information,
            [CallerMemberName] string callerName = "")
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));

            var fullName = LogCallOptions.GetCallerFullName(caller, callerName);

#pragma warning disable CA2254 // Template should be a static expression - will be statically initialized
            logger.Log(logLevel, LogCallOptions.Instance.LogTemplateWithNoArguments, fullName);
#pragma warning restore CA2254 // Template should be a static expression
        }

        /// <summary>Logs a method call and its arguments.</summary>
        /// <param name="logger">The logger used to log the method call.</param>
        /// <param name="caller">The object instance being logged. Use <see langword="null"/> for static methods.</param>
        /// <param name="arguments">An object containing the arguments to be included in the structured log message.</param>
        /// <param name="logLevel">The logging level.</param>
        /// <param name="callerName">The name of the method being logged. When not used, the called method's name will be used.</param>
        /// <remarks>See <see cref="LogCallOptions.UseCallPrefix(string)"/> and <see cref="LogCallOptions.UseMethodNameProperty(string)"/>
        /// and <see cref="LogCallOptions.UseArgumentsDestructureProperty(string)"/> to change the format of the message template.</remarks>
        public static void LogCall(this ILogger logger, object? caller, object arguments, LogLevel logLevel = LogLevel.Information,
            [CallerMemberName] string callerName = "")
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = arguments.WhenNotNull(nameof(arguments));
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));

            var fullName = LogCallOptions.GetCallerFullName(caller, callerName);

#pragma warning disable CA2254 // Template should be a static expression - will be statically initialized
            logger.Log(logLevel, LogCallOptions.Instance.LogTemplateWithArguments, fullName, arguments);
#pragma warning restore CA2254 // Template should be a static expression
        }

        /// <summary>Logs an exception with a logging level of <see cref="LogLevel.Error"/>.</summary>
        /// <param name="logger">The logger used to log the method call.</param>
        /// <param name="exception">The exception containing the message to be logged.</param>
        /// <param name="messageTemplate">An optional message template to be used. The default is '<c>Error: {ErrorMessage}</c>'.</param>
        /// <param name="arguments">Additional arguments that can be provided when the <paramref name="messageTemplate"/> options is used.</param>
        /// <remarks>See <see cref="LogCallOptions.UseExceptionPrefix(string)"/> and <see cref="LogCallOptions.UseExceptionMessageProperty(string)"/>
        /// to change the format of the message template.</remarks>
        public static void LogException(this ILogger logger, Exception exception, string? messageTemplate = default,
            params object?[] arguments)
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = exception.WhenNotNull(nameof(exception));

            if (messageTemplate is null)
            {
#pragma warning disable CA2254 // Template should be a static expression - will be statically initialized
                logger.LogError(exception, LogCallOptions.Instance.LogExceptionTemplate, exception.Message);
#pragma warning restore CA2254 // Template should be a static expression
            }
            else
            {
#pragma warning disable CA2254 // Template should be a static expression - will be statically initialized
                logger.LogError(exception, messageTemplate, arguments);
#pragma warning restore CA2254 // Template should be a static expression
            }
        }

        /// <summary>Logs the provided exception message (using the <paramref name="messageTemplate"/> and <paramref name="arguments"/> if provided)
        /// as well as its' nested inner exceptions. The inner exception messages are logged using the default message template, '<c>Error: {ErrorMessage}</c>'.</summary>
        /// <param name="logger">The logger used to log the method call.</param>
        /// <param name="exception">The exception containing the message to be logged.</param>
        /// <param name="messageTemplate">An optional message template to be used. The default is '<c>Error: {ErrorMessage}</c>'.</param>
        /// <param name="arguments">Additional arguments that can be provided when the <paramref name="messageTemplate"/> options is used.</param>
        /// <remarks>See <see cref="LogCallOptions.UseExceptionPrefix(string)"/> and <see cref="LogCallOptions.UseExceptionMessageProperty(string)"/>
        /// to change the format of the message template.</remarks>
        public static void LogAllExceptions(this ILogger logger, Exception exception, string? messageTemplate = default,
            params object?[] arguments)
        {
            _ = logger.WhenNotNull(nameof(logger));
            _ = exception.WhenNotNull(nameof(exception));

            logger.LogException(exception, messageTemplate, arguments);

            var innerException = exception.InnerException;

            while (innerException is not null)
            {
                logger.LogException(innerException);

                innerException = innerException.InnerException;
            }
        }
    }
}
