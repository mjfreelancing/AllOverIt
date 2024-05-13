#nullable enable

using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing
{
    public static class MethodCallsWithArgumentsExtensions
    {
        public static void AssertStaticLogCallEntry(this MethodCallContext methodCallContext, int index, string callerMethodName,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCallEntries(callerMethodName);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Entries[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        public static void AssertStaticLogCallWithArgumentsEntry(this MethodCallContext methodCallContext, int index,
            string callerMethodName, object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsEntries(callerMethodName, arguments);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Entries[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        public static void AssertLogCallEntry<TCaller>(this MethodCallContext methodCallContext, int index, string callerMethodName,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogCallEntries<TCaller>(callerMethodName);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Entries[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        public static void AssertLogCallWithArgumentsEntry<TCaller>(this MethodCallContext methodCallContext, int index,
            string callerMethodName, object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogCallWithArgumentsEntries<TCaller>(callerMethodName, arguments);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Entries[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        public static void AssertExceptionLogEntry(this MethodCallContext methodCallContext, int index, Exception exception)
        {
            var expected = LogCallExpectation.GetExpectedExceptionLogEntries(exception);

            methodCallContext.LogLevels[index].Should().Be(LogLevel.Error);
            methodCallContext.Entries[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeSameAs(exception);
        }

        public static void AssertMessageEntry(this MethodCallContext methodCallContext, int index, string message,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogTemplateEntries(message);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Entries[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        public static void AssertMessageWithArgumentsEntry(this MethodCallContext methodCallContext, int index, string logTemplate,
            object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogTemplateWithArgumentsEntries(logTemplate, arguments);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Entries[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }
    }
}