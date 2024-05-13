#nullable enable

using FluentAssertions;

namespace AllOverIt.Logging.Testing
{
    public static class MethodCallsWithArgumentsExtensions
    {
        public static void AssertStaticLogCallEntry(this MethodCallsWithArguments methodCallsWithArguments, int index, string callerMethodName)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCall(callerMethodName);

            methodCallsWithArguments.States[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertStaticLogCallWithArgumentsEntry(this MethodCallsWithArguments methodCallsWithArguments, int index,
            string callerMethodName, object arguments)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCallWithArguments(callerMethodName, arguments);

            methodCallsWithArguments.States[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertLogCallEntry<TCaller>(this MethodCallsWithArguments methodCallsWithArguments, int index, string callerMethodName)
        {
            var expected = LogCallExpectation.GetExpectedLogCall<TCaller>(callerMethodName);

            methodCallsWithArguments.States[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertLogCallWithArgumentsEntry<TCaller>(this MethodCallsWithArguments methodCallsWithArguments, int index,
            string callerMethodName, object arguments)
        {
            var expected = LogCallExpectation.GetExpectedLogCallWithArguments<TCaller>(callerMethodName, arguments);

            methodCallsWithArguments.States[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeNull();
        }

        public static void AssertExceptionLogEntry(this MethodCallsWithArguments methodCallsWithArguments, int index, Exception exception)
        {
            var expected = LogCallExpectation.GetExpectedExceptionLogEntry(exception);

            methodCallsWithArguments.States[index].Should().BeEquivalentTo(expected);
            methodCallsWithArguments.Exceptions[index].Should().BeSameAs(exception);
        }
    }
}