#nullable enable

using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing
{
    public static class MethodCallsWithArgumentsExtensions
    {
        public static void AssertStaticLogCallEntry(this MethodCallsWithArguments methodCallsWithArguments, int index, string callerMethodName,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCallEntries(callerMethodName);

            methodCallsWithArguments.LogLevels[index].Should().Be(logLevel);
            methodCallsWithArguments.Entries[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertStaticLogCallWithArgumentsEntry(this MethodCallsWithArguments methodCallsWithArguments, int index,
            string callerMethodName, object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsEntries(callerMethodName, arguments);

            methodCallsWithArguments.LogLevels[index].Should().Be(logLevel);
            methodCallsWithArguments.Entries[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertLogCallEntry<TCaller>(this MethodCallsWithArguments methodCallsWithArguments, int index, string callerMethodName,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogCallEntries<TCaller>(callerMethodName);

            methodCallsWithArguments.LogLevels[index].Should().Be(logLevel);
            methodCallsWithArguments.Entries[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertLogCallWithArgumentsEntry<TCaller>(this MethodCallsWithArguments methodCallsWithArguments, int index,
            string callerMethodName, object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogCallWithArgumentsEntries<TCaller>(callerMethodName, arguments);

            methodCallsWithArguments.LogLevels[index].Should().Be(logLevel);
            methodCallsWithArguments.Entries[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertExceptionLogEntry(this MethodCallsWithArguments methodCallsWithArguments, int index, Exception exception)
        {
            var expected = LogCallExpectation.GetExpectedExceptionLogEntries(exception);

            methodCallsWithArguments.LogLevels[index].Should().Be(LogLevel.Error);
            methodCallsWithArguments.Entries[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeSameAs(exception);
        }

        public static void AssertMessageEntry(this MethodCallsWithArguments methodCallsWithArguments, int index, string message,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogTemplateEntries(message);

            methodCallsWithArguments.LogLevels[index].Should().Be(logLevel);
            methodCallsWithArguments.Entries[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertMessageWithArgumentsEntry(this MethodCallsWithArguments methodCallsWithArguments, int index, string logTemplate,
            object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogTemplateWithArgumentsEntries(logTemplate, arguments);

            methodCallsWithArguments.LogLevels[index].Should().Be(logLevel);
            methodCallsWithArguments.Entries[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }
    }
}