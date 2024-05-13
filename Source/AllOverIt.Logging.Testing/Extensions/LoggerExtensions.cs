using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AllOverIt.Logging.Testing.Extensions
{
    public static class LoggerExtensions
    {
        public static void AssertStaticLogCall(this ILogger loggerFake, Action action, string callerMethodName)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertStaticLogCallEntry(0, callerMethodName);
        }

        public static void AssertStaticLogCallWithArguments(this ILogger loggerFake, Action action,
            string callerMethodName, object arguments)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertStaticLogCallWithArgumentsEntry(0, callerMethodName, arguments);
        }

        public static void AssertLogCall<TCaller>(this ILogger loggerFake, Action action, string callerMethodName)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertLogCallEntry<TCaller>(0, callerMethodName);
        }

        public static void AssertLogCallWithArguments<TCaller>(this ILogger loggerFake, Action action,
            string callerMethodName, object arguments)
        {
            loggerFake
                .CaptureLogCalls(action)
                .AssertLogCallWithArgumentsEntry<TCaller>(0, callerMethodName, arguments);
        }

        public static async Task AssertLogCallAsync<TCaller>(this ILogger loggerFake, Func<Task> action, string callerMethodName)
        {
            var actual = await loggerFake.CaptureLogCallsAsync(action);

            actual.AssertLogCallEntry<TCaller>(0, callerMethodName);
        }

        public static async Task AssertLogCallWithArgumentsAsync<TCaller>(this ILogger loggerFake, Func<Task> action, string callerMethodName,
            object arguments)
        {
            var actual = await loggerFake.CaptureLogCallsAsync(action);

            actual.AssertLogCallWithArgumentsEntry<TCaller>(0, callerMethodName, arguments);
        }

        public static MethodCallsWithArguments CaptureLogCalls(this ILogger loggerFake, Action action)
        {
            var actualLogCallArgs = new MethodCallsWithArguments();

            loggerFake
                .When(call => call.Log(Arg.Any<LogLevel>(),
                                       Arg.Any<EventId>(),
                                       Arg.Any<object>(),
                                       Arg.Any<Exception>(),
                                       Arg.Any<Func<object, Exception, string>>()))
                .Do(args =>
                {
                    var keyValues = ((IReadOnlyList<KeyValuePair<string, object>>) args[2])
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    // protected against multiple threads trying to update actualLogCallArgs
                    lock (actualLogCallArgs)
                    {
                        actualLogCallArgs.Add((keyValues, (Exception) args[3]));
                    }
                });

            action.Invoke();

            return actualLogCallArgs;
        }

        public static async Task<MethodCallsWithArguments> CaptureLogCallsAsync(this ILogger loggerFake, Func<Task> action)
        {
            var actualLogCallArgs = new MethodCallsWithArguments();

            loggerFake
                .When(call => call.Log(Arg.Any<LogLevel>(),
                                       Arg.Any<EventId>(),
                                       Arg.Any<object>(),
                                       Arg.Any<Exception>(),
                                       Arg.Any<Func<object, Exception, string>>()))
                .Do(args =>
                {
                    var keyValues = ((IReadOnlyList<KeyValuePair<string, object>>) args[2])
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    // protected against multiple threads trying to update actualLogCallArgs
                    lock (actualLogCallArgs)
                    {
                        actualLogCallArgs.Add((keyValues, (Exception) args[3]));
                    }
                });

            await action.Invoke();

            return actualLogCallArgs;
        }
    }
}