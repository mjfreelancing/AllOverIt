using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Logging.Testing
{
    internal static class LogCallExpectation
    {
        private const string OriginalFormat = "{OriginalFormat}";

        private static readonly string MethodNameProperty = LogCallOptions.Instance._methodNameProperty;
        private static readonly string ArgumentsProperty = LogCallOptions.Instance._argumentsDestructureProperty;

        public static IDictionary<string, object> GetExpectedStaticLogCallMetadata(string callerName)
        {
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));

            return new Dictionary<string, object>
            {
                { MethodNameProperty, callerName },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedLogCallMetadata<TCaller>(string callerName)
        {
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));

            var callerType = typeof(TCaller);

            return new Dictionary<string, object>
            {
                { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedStaticLogCallWithArgumentsMetadata(string callerName, object arguments)
        {
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));
            _ = arguments.WhenNotNull(nameof(arguments));

            return new Dictionary<string, object>
            {
                { MethodNameProperty, callerName},
                { $"@{ArgumentsProperty}", arguments },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedLogCallWithArgumentsMetadata<TCaller>(string callerName, object arguments)
        {
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));
            _ = arguments.WhenNotNull(nameof(arguments));

            var callerType = typeof(TCaller);

            return new Dictionary<string, object>
            {
                { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                { $"@{ArgumentsProperty}", arguments },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedExceptionMetadata(Exception exception)
        {
            _ = exception.WhenNotNull(nameof(exception));

            return new Dictionary<string, object>
            {
                { LogCallOptions.Instance._exceptionMessageProperty, exception.Message },
                { OriginalFormat, LogCallOptions.Instance.LogExceptionTemplate }
            };
        }

        public static IDictionary<string, object> GetExpectedLogMessageMetadata(string message)
        {
            _ = message.WhenNotNullOrEmpty(nameof(message));

            return new Dictionary<string, object>
            {
                { OriginalFormat, message }
            };
        }

        public static IDictionary<string, object> GetExpectedLogTemplateWithArgumentsMetadata(string logTemplate, object arguments)
        {
            _ = logTemplate.WhenNotNullOrEmpty(nameof(logTemplate));
            _ = arguments.WhenNotNull(nameof(arguments));

            var dictionary = arguments.ToPropertyDictionary();

            dictionary.Add(OriginalFormat, logTemplate);

            return dictionary;
        }
    }
}