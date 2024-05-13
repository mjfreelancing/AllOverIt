using AllOverIt.Extensions;

namespace AllOverIt.Logging.Testing
{
    internal static class LogCallExpectation
    {
        private const string OriginalFormat = "{OriginalFormat}";

        private static readonly string MethodNameProperty = LogCallOptions.Instance._methodNameProperty;
        private static readonly string ArgumentsProperty = LogCallOptions.Instance._argumentsProperty;

        public static IDictionary<string, object> GetExpectedStaticLogCall(string callerName)
        {
            return new Dictionary<string, object>
            {
                { MethodNameProperty, callerName },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedLogCall<TCaller>(string callerName)
        {
            var callerType = typeof(TCaller);

            return new Dictionary<string, object>
            {
                { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedStaticLogCallWithArguments(string callerName, object arguments)
        {
            return new Dictionary<string, object>
            {
                { MethodNameProperty, callerName},
                { $"@{ArgumentsProperty}", arguments },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedLogCallWithArguments<TCaller>(string callerName, object arguments)
        {
            var callerType = typeof(TCaller);

            return new Dictionary<string, object>
            {
                { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                { $"@{ArgumentsProperty}", arguments },
                { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
            };
        }

        public static IDictionary<string, object> GetExpectedExceptionLogEntry(Exception exception)
        {
            return new Dictionary<string, object>
            {
                { LogCallOptions.Instance._exceptionMessageProperty, exception.Message },
                { OriginalFormat, LogCallOptions.Instance.LogExceptionTemplate }
            };
        }

        public static IDictionary<string, object> GetExpectedLogEntryWithTemplateAndArguments(string logTemplate, object arguments)
        {
            var dictionary = arguments.ToPropertyDictionary();

            dictionary.Add(OriginalFormat, logTemplate);

            return dictionary;
        }
    }
}