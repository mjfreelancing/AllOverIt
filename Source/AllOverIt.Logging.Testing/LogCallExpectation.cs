using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Logging.Testing
{
    /// <summary>Contains helper methods to get expected logging metadata.</summary>
    public static class LogCallExpectation
    {
        private const string OriginalFormat = "{OriginalFormat}";

        private static readonly string MethodNameProperty = LogCallOptions.Instance._methodNameProperty;
        private static readonly string ArgumentsProperty = LogCallOptions.Instance._argumentsDestructureProperty;

        /// <summary>Gets the expected metadata following a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, Microsoft.Extensions.Logging.LogLevel, string)"/>.</summary>
        /// <param name="callerName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, Microsoft.Extensions.Logging.LogLevel, string)"/>.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object> GetExpectedStaticLogCallMetadata(string callerName)
        {
            _ = callerName.WhenNotNullOrEmpty();

            return new Dictionary<string, object>
            {
                { MethodNameProperty, callerName },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
            };
        }

        /// <summary>Gets the expected metadata following a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, Microsoft.Extensions.Logging.LogLevel, string)"/>.</summary>
        /// <typeparam name="TCaller">The object instance calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, Microsoft.Extensions.Logging.LogLevel, string)"/>.</typeparam>
        /// <param name="callerName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, Microsoft.Extensions.Logging.LogLevel, string)"/>.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object> GetExpectedLogCallMetadata<TCaller>(string callerName)
        {
            _ = callerName.WhenNotNullOrEmpty();

            var callerType = typeof(TCaller);

            return new Dictionary<string, object>
            {
                { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
            };
        }

        /// <summary>Gets the expected metadata following a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, object, Microsoft.Extensions.Logging.LogLevel, string)"/>.</summary>
        /// <param name="callerName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, object, Microsoft.Extensions.Logging.LogLevel, string)"/>.</param>
        /// <param name="arguments">The arguments expected to be captured.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object> GetExpectedStaticLogCallWithArgumentsMetadata(string callerName, object arguments)
        {
            _ = callerName.WhenNotNullOrEmpty();
            _ = arguments.WhenNotNull();

            return new Dictionary<string, object>
            {
                { MethodNameProperty, callerName},
                { $"@{ArgumentsProperty}", arguments },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
            };
        }

        /// <summary>Gets the expected metadata following a call to <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, object, Microsoft.Extensions.Logging.LogLevel, string)"/>.</summary>
        /// <typeparam name="TCaller">The object instance calling <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, object, Microsoft.Extensions.Logging.LogLevel, string)"/>.</typeparam>
        /// <param name="callerName">The name of the method expected to call <see cref="Logging.Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, object, Microsoft.Extensions.Logging.LogLevel, string)"/>.</param>
        /// <param name="arguments">The arguments expected to be captured.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object> GetExpectedLogCallWithArgumentsMetadata<TCaller>(string callerName, object arguments)
        {
            _ = callerName.WhenNotNullOrEmpty();
            _ = arguments.WhenNotNull();

            var callerType = typeof(TCaller);

            return new Dictionary<string, object>
            {
                { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                { $"@{ArgumentsProperty}", arguments },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
            };
        }

        /// <summary>Gets the expected metadata following a call to <see cref="Logging.Extensions.LoggerExtensions.LogException(Microsoft.Extensions.Logging.ILogger, Exception, string?, object?[])"/>.</summary>
        /// <param name="exception">The exception expected to be captured.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object> GetExpectedExceptionMetadata(Exception exception)
        {
            _ = exception.WhenNotNull();

            return new Dictionary<string, object>
            {
                { LogCallOptions.Instance._exceptionMessageProperty, exception.Message },
                { OriginalFormat, LogCallOptions.Instance.LogExceptionTemplate }
            };
        }

        /// <summary>Gets the expected metadata following a call to log a message.</summary>
        /// <param name="message">The message expected to be captured.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object> GetExpectedLogMessageMetadata(string message)
        {
            _ = message.WhenNotNullOrEmpty();

            return new Dictionary<string, object>
            {
                { OriginalFormat, message }
            };
        }

        /// <summary>Gets the expected metadata following a call to log a message using a log template with arguments.</summary>
        /// <param name="logTemplate">The log template expected to be captured.</param>
        /// <param name="arguments">The arguments expected to be captured.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object?> GetExpectedLogTemplateWithArgumentsMetadata(string logTemplate, object arguments)
        {
            _ = logTemplate.WhenNotNullOrEmpty();
            _ = arguments.WhenNotNull();

            return GetExpectedLogTemplateWithArgumentsMetadata(logTemplate, arguments.ToPropertyDictionary());
        }

        /// <summary>Gets the expected metadata following a call to log a message using a log template with arguments.</summary>
        /// <param name="logTemplate">The log template expected to be captured.</param>
        /// <param name="arguments">The arguments, as a <c>Dictionary&lt;string, object&gt;</c>, expected to be captured.</param>
        /// <returns>The expected metadata.</returns>
        public static IDictionary<string, object?> GetExpectedLogTemplateWithArgumentsMetadata(string logTemplate, IDictionary<string, object?> arguments)
        {
            _ = logTemplate.WhenNotNullOrEmpty();
            _ = arguments.WhenNotNull();

            return new Dictionary<string, object?>(arguments)
            {
                { OriginalFormat, logTemplate }
            };
        }
    }
}