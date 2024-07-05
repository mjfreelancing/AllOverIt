#nullable enable

using AllOverIt;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing.Extensions
{
    /// <summary>Contains extensions methods for <see cref="MethodCallContext"/> to help with asserting captured logging data.</summary>
    public static class MethodCallsWithArgumentsExtensions
    {
        /// <summary>Asserts that a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/> was made
        /// during the invocation of a static method.</summary>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="callerMethodName">The expected name of the static method called.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertStaticLogCallEntry(this MethodCallContext methodCallContext, int index, string callerMethodName, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCallMetadata(callerMethodName);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        /// <summary>Asserts that a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>
        /// was made during the invocation of a static method.</summary>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="callerMethodName">The expected name of the static method called.</param>
        /// <param name="arguments">The arguments, as an object, expected to be included with the logging information.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertStaticLogCallWithArgumentsEntry(this MethodCallContext methodCallContext, int index, string callerMethodName,
            object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsMetadata(callerMethodName, arguments);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        /// <summary>Asserts that a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/> was made
        /// during the invocation of a non-static method.</summary>
        /// <typeparam name="TCaller">The object type calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</typeparam>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="callerMethodName">The expected name of the static method called.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertLogCallEntry<TCaller>(this MethodCallContext methodCallContext, int index, string callerMethodName, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogCallMetadata<TCaller>(callerMethodName);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        /// <summary>Asserts that a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/> was made
        /// during the invocation of a non-static method.</summary>
        /// <typeparam name="TCaller">The object type calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>.</typeparam>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="callerMethodName">The expected name of the static method called.</param>
        /// <param name="arguments">The arguments, as an object, expected to be included with the logging information.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertLogCallWithArgumentsEntry<TCaller>(this MethodCallContext methodCallContext, int index, string callerMethodName,
            object arguments, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogCallWithArgumentsMetadata<TCaller>(callerMethodName, arguments);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        /// <summary>Asserts that a call to <see cref="Logging.Extensions.LoggerExtensions.LogException(ILogger, Exception, string?, object?[])"/> was made,
        /// without a log template or arguments.
        /// The method also asserts the logging level was <see cref="LogLevel.Error"/>.</summary>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="exception">The exception expected to be included with the log information.</param>
        public static void AssertExceptionLogEntry(this MethodCallContext methodCallContext, int index, Exception exception)
        {
            var expected = LogCallExpectation.GetExpectedExceptionMetadata(exception);

            methodCallContext.LogLevels[index].Should().Be(LogLevel.Error);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeEquivalentTo(exception);
        }

        /// <summary>Asserts that a call to <see cref="Logging.Extensions.LoggerExtensions.LogException(ILogger, Exception, string?, object?[])"/> was made.
        /// The method also asserts the logging level was <see cref="LogLevel.Error"/>.</summary>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="exception">The exception expected to be included with the log information.</param>
        /// <param name="logTemplate">The expected log template to be included with the log information.</param>
        /// <param name="arguments">The arguments, as an object, expected to be included with the logging information.</param>
        public static void AssertExceptionWithArgumentsLogEntry(this MethodCallContext methodCallContext, int index, Exception exception, string logTemplate,
            object arguments)
        {
            var expected = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata(logTemplate, arguments);

            methodCallContext.LogLevels[index].Should().Be(LogLevel.Error);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeEquivalentTo(exception);
        }

        /// <summary>Asserts that a log message at a specified logging level was captured.</summary>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="message">The expected log message.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertMessageEntry(this MethodCallContext methodCallContext, int index, string message, LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogMessageMetadata(message);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        /// <summary>Asserts that a logging template and arguments at a specified logging level was captured.</summary>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="logTemplate">The expecting logging template.</param>
        /// <param name="arguments">The expected arguments, as an object, expected to be included with the logging information.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertMessageWithArgumentsEntry(this MethodCallContext methodCallContext, int index, string logTemplate, object arguments,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata(logTemplate, arguments);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }

        /// <summary>Asserts that a logging template and arguments at a specified logging level was captured.</summary>
        /// <param name="methodCallContext">The context containing logger information collected during the invocation of one or more methods.</param>
        /// <param name="index">The index within the collection of gathered information.</param>
        /// <param name="logTemplate">The expecting logging template.</param>
        /// <param name="arguments">The expected arguments, as a <c>Dictionary&lt;string, object&gt;</c>, expected to be included with the logging information.</param>
        /// <param name="logLevel">The expected logging level.</param>
        public static void AssertMessageWithArgumentsEntry(this MethodCallContext methodCallContext, int index, string logTemplate, IDictionary<string, object> arguments,
            LogLevel logLevel)
        {
            var expected = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata(logTemplate, arguments);

            methodCallContext.LogLevels[index].Should().Be(logLevel);
            methodCallContext.Metadata[index].Should().BeEquivalentTo(expected);
            methodCallContext.Exceptions[index].Should().BeNull();
        }
    }
}