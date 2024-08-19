using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AllOverIt.Logging.Testing.Extensions
{
    /// <summary>Contains extensions methods for <see cref="ILogger"/> substitutes created by <c>NSubstitute</c>.</summary>
    public static class LoggerExtensions
    {
        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>
        /// in a static method.</summary>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertStaticLogCall(this ILogger logger, Action action, string callerMethodName, LogLevel logLevel)
        {
            logger
                .CaptureLogCalls(action)
                .AssertStaticLogCallEntry(0, callerMethodName, logLevel);
        }

        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>
        /// in a static method.</summary>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The asynchronous action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static async Task AssertStaticLogCallAsync(this ILogger logger, Func<Task> action, string callerMethodName, LogLevel logLevel)
        {
            var methodCallContext = await logger.CaptureLogCallsAsync(action);

            methodCallContext.AssertStaticLogCallEntry(0, callerMethodName, logLevel);
        }

        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>
        /// in a static method.</summary>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</param>
        /// <param name="arguments">Additional arguments, as an object, expected to be logged.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertStaticLogCallWithArguments(this ILogger logger, Action action, string callerMethodName, object arguments, LogLevel logLevel)
        {
            logger
                .CaptureLogCalls(action)
                .AssertStaticLogCallWithArgumentsEntry(0, callerMethodName, arguments, logLevel);
        }

        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>
        /// in a static method.</summary>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The asynchronous action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</param>
        /// <param name="arguments">Additional arguments, as an object, expected to be logged.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static async Task AssertStaticLogCallWithArgumentsAsync(this ILogger logger, Func<Task> action, string callerMethodName, object arguments, LogLevel logLevel)
        {
            var methodCallContext = await logger.CaptureLogCallsAsync(action);

            methodCallContext.AssertStaticLogCallWithArgumentsEntry(0, callerMethodName, arguments, logLevel);
        }

        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>
        /// in a non-static method.</summary>
        /// <typeparam name="TCaller">The object type calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</typeparam>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertLogCall<TCaller>(this ILogger logger, Action action, string callerMethodName, LogLevel logLevel)
        {
            logger
                .CaptureLogCalls(action)
                .AssertLogCallEntry<TCaller>(0, callerMethodName, logLevel);
        }

        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>
        /// in a non-static method.</summary>
        /// <typeparam name="TCaller">The object type calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>.</typeparam>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>.</param>
        /// <param name="arguments">Additional arguments, as an object, expected to be logged.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertLogCallWithArguments<TCaller>(this ILogger logger, Action action, string callerMethodName, object arguments, LogLevel logLevel)
        {
            logger
                .CaptureLogCalls(action)
                .AssertLogCallWithArgumentsEntry<TCaller>(0, callerMethodName, arguments, logLevel);
        }

        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>
        /// in a non-static method.</summary>
        /// <typeparam name="TCaller">The object type calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</typeparam>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The asynchronous action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static async Task AssertLogCallAsync<TCaller>(this ILogger logger, Func<Task> action, string callerMethodName, LogLevel logLevel)
        {
            var methodCallContext = await logger.CaptureLogCallsAsync(action);

            methodCallContext.AssertLogCallEntry<TCaller>(0, callerMethodName, logLevel);
        }

        /// <summary>Asserts that a log was entry was made via a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>
        /// in a non-static method.</summary>
        /// <typeparam name="TCaller">The object type calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>.</typeparam>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The asynchronous action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <param name="callerMethodName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>.</param>
        /// <param name="arguments">Additional arguments, as an object, expected to be logged.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static async Task AssertLogCallWithArgumentsAsync<TCaller>(this ILogger logger, Func<Task> action, string callerMethodName,
            object arguments, LogLevel logLevel)
        {
            var methodCallContext = await logger.CaptureLogCallsAsync(action);

            methodCallContext.AssertLogCallWithArgumentsEntry<TCaller>(0, callerMethodName, arguments, logLevel);
        }

        /// <summary>Captures logging information gathered during the invocation of an action. Refer to the extension methods available for <see cref="MethodCallContext"/>
        /// that simplify how assertions can be made.</summary>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <returns>A <see cref="MethodCallContext"/> containing information about all logging methods invoked.</returns>
        public static MethodCallContext CaptureLogCalls(this ILogger logger, Action action)
        {
            // The methodCallContext will be populated as the logger's methods are invoked
            var methodCallContext = PrepareMethodCallsWithArguments(logger);

            action.Invoke();

            return methodCallContext;
        }

        /// <summary>Captures logging information gathered during the invocation of an action. Refer to the extension methods available for <see cref="MethodCallContext"/>
        /// that simplify how assertions can be made.</summary>
        /// <param name="logger">The logger substitute.</param>
        /// <param name="action">The asynchronous action to be invoked that will result in logging methods being called on the <paramref name="logger"/>.</param>
        /// <returns>A <see cref="MethodCallContext"/> containing information about all logging methods invoked.</returns>
        public static async Task<MethodCallContext> CaptureLogCallsAsync(this ILogger logger, Func<Task> action)
        {
            // The methodCallContext will be populated as the logger's methods are invoked
            var methodCallContext = PrepareMethodCallsWithArguments(logger);

            await action.Invoke();

            return methodCallContext;
        }

        private static MethodCallContext PrepareMethodCallsWithArguments(ILogger logger)
        {
            var methodCallContext = new MethodCallContext();

            logger
                .When(call => call.Log(Arg.Any<LogLevel>(),
                                       Arg.Any<EventId>(),
                                       Arg.Any<object>(),
                                       Arg.Any<Exception?>(),
                                       Arg.Any<Func<object, Exception?, string>>()))
                .Do(args =>
                {
                    lock (methodCallContext)
                    {
                        var callContext = new MethodCallContext.Item
                        {
                            LogLevel = args.ArgAt<LogLevel>(0),

                            // Is actually of type FormattedLogValues, but this isn't public
                            Metadata = args.ArgAt<IReadOnlyList<KeyValuePair<string, object?>>>(2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                            Exception = args.ArgAt<Exception?>(3)
                        };

                        methodCallContext.Add(callContext);
                    }
                });

            return methodCallContext;
        }
    }
}