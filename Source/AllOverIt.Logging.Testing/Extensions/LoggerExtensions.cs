using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AllOverIt.Logging.Testing.Extensions
{
    public static class LoggerExtensions
    {
        public static void AssertStaticLogCall(this ILogger loggerFake, Action action, string callerMethodName, LogLevel logLevel)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertStaticLogCallEntry(0, callerMethodName, logLevel);
        }

        public static void AssertStaticLogCallWithArguments(this ILogger loggerFake, Action action,
            string callerMethodName, object arguments, LogLevel logLevel)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertStaticLogCallWithArgumentsEntry(0, callerMethodName, arguments, logLevel);
        }

        public static void AssertLogCall<TCaller>(this ILogger loggerFake, Action action, string callerMethodName, LogLevel logLevel)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertLogCallEntry<TCaller>(0, callerMethodName, logLevel);
        }

        public static void AssertLogCallWithArguments<TCaller>(this ILogger loggerFake, Action action,
            string callerMethodName, object arguments, LogLevel logLevel)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertLogCallWithArgumentsEntry<TCaller>(0, callerMethodName, arguments, logLevel);
        }

        public static async Task AssertLogCallAsync<TCaller>(this ILogger loggerFake, Func<Task> action, string callerMethodName, LogLevel logLevel)
        {
            var actual = await loggerFake.CaptureLogCallsAsync(action);

            actual.AssertLogCallEntry<TCaller>(0, callerMethodName, logLevel);
        }

        public static async Task AssertLogCallWithArgumentsAsync<TCaller>(this ILogger loggerFake, Func<Task> action, string callerMethodName,
            object arguments, LogLevel logLevel)
        {
            var actual = await loggerFake.CaptureLogCallsAsync(action);

            actual.AssertLogCallWithArgumentsEntry<TCaller>(0, callerMethodName, arguments, logLevel);
        }

        public static MethodCallContext CaptureLogCalls(this ILogger loggerFake, Action action)
        {
            // The methodCallContext will be populated as the logger's methods are invoked
            var methodCallContext = PrepareMethodCallsWithArguments(loggerFake);

            action.Invoke();

            return methodCallContext;
        }

        public static async Task<MethodCallContext> CaptureLogCallsAsync(this ILogger loggerFake, Func<Task> action)
        {
            // The methodCallContext will be populated as the logger's methods are invoked
            var methodCallContext = PrepareMethodCallsWithArguments(loggerFake);

            await action.Invoke();

            return methodCallContext;
        }

        private static MethodCallContext PrepareMethodCallsWithArguments(ILogger loggerFake)
        {
            var actualLogCallArgs = new MethodCallContext();

            loggerFake
                .When(call => call.Log(Arg.Any<LogLevel>(),
                                       Arg.Any<EventId>(),
                                       Arg.Any<object>(),
                                       Arg.Any<Exception>(),
                                       Arg.Any<Func<object, Exception, string>>()))
                .Do(args =>
                {
                    lock (actualLogCallArgs)
                    {
                        var callContext = new MethodCallContext.Item
                        {
                            LogLevel = args.ArgAt<LogLevel>(0),
                            Entries = args.ArgAt<IReadOnlyList<KeyValuePair<string, object>>>(2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                            Exception = args.ArgAt<Exception>(3)
                        };

                        actualLogCallArgs.Add(callContext);
                    }
                });

            return actualLogCallArgs;
        }
    }
}