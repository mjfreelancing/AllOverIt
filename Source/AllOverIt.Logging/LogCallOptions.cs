#nullable enable

using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Logging
{
    public sealed class LogCallOptions
    {
        private string? _logTemplateWithNoArguments = null;     // "Call: {MethodName}";
        private string? _logTemplateWithArguments = null;       // "Call: {MethodName}, Arguments = {@Arguments}";
        private string? _logExceptionTemplate = null;           // "Error: {ErrorMessage}";
        private bool _includeCallerNamespace = true;

        internal static readonly LogCallOptions Instance = new();

        internal string _callPrefix = "Call: ";
        internal string _exceptionPrefix = "Error: ";
        internal string _methodNameProperty = "MethodName";
        internal string _argumentsPrefix = "Arguments = ";
        internal string _argumentsProperty = "Arguments";
        internal string _exceptionMessageProperty = "ErrorMessage";

        internal string LogTemplateWithNoArguments
        {
            get
            {
                _logTemplateWithNoArguments ??= $"{_callPrefix}{{{_methodNameProperty}}}";

                return _logTemplateWithNoArguments;
            }
        }

        internal string LogTemplateWithArguments
        {
            get
            {
                _logTemplateWithArguments ??= $"{_callPrefix}{{{_methodNameProperty}}}, {_argumentsPrefix}{{@{_argumentsProperty}}}";

                return _logTemplateWithArguments;
            }
        }

        internal string LogExceptionTemplate
        {
            get
            {
                _logExceptionTemplate ??= $"{_exceptionPrefix}{{{_exceptionMessageProperty}}}";

                return _logExceptionTemplate;
            }
        }

        public static LogCallOptions UseCallPrefix(string callPrefix)
        {
            Instance._callPrefix = callPrefix.WhenNotNullOrEmpty(nameof(callPrefix));

            Instance._logTemplateWithNoArguments = null;
            Instance._logTemplateWithArguments = null;

            return Instance;
        }

        public static LogCallOptions UseExceptionPrefix(string exceptionPrefix)
        {
            Instance._exceptionPrefix = exceptionPrefix.WhenNotNullOrEmpty(nameof(exceptionPrefix));

            Instance._logExceptionTemplate = null;

            return Instance;
        }

        public static LogCallOptions UseMethodNameProperty(string methodNameProperty)
        {
            Instance._methodNameProperty = methodNameProperty.WhenNotNullOrEmpty(nameof(methodNameProperty));

            Instance._logTemplateWithNoArguments = null;
            Instance._logTemplateWithArguments = null;

            return Instance;
        }

        public static LogCallOptions UseExceptionMessageProperty(string exceptionMessageProperty)
        {
            Instance._exceptionMessageProperty = exceptionMessageProperty.WhenNotNullOrEmpty(nameof(exceptionMessageProperty));

            Instance._logExceptionTemplate = null;

            return Instance;
        }

        public static LogCallOptions UseArgumentsPrefix(string argumentsPrefix)
        {
            Instance._argumentsPrefix = argumentsPrefix.WhenNotNullOrEmpty(nameof(argumentsPrefix));

            Instance._logTemplateWithArguments = null;

            return Instance;
        }

        public static LogCallOptions UseArgumentsDestructureProperty(string argumentsProperty)
        {
            Instance._argumentsProperty = argumentsProperty.WhenNotNullOrEmpty(nameof(argumentsProperty));

            Instance._logTemplateWithArguments = null;

            return Instance;
        }

        public static LogCallOptions IncludeCallerNamespace(bool includeCallerNamespace)
        {
            Instance._includeCallerNamespace = includeCallerNamespace;

            return Instance;
        }

        public static string GetCallerFullName(object? caller, string callerName)
        {
            if (caller is null)
            {
                return callerName;
            }

            var callerType = caller.GetType();
            var callerTypeMethodName = $"{callerType.GetFriendlyName()}.{callerName}";

            if (!Instance._includeCallerNamespace || callerType.Namespace is null)
            {
                return callerTypeMethodName;
            }

            return $"{callerType.Namespace}.{callerTypeMethodName}";
        }
    }
}
