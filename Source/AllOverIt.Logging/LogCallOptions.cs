#nullable enable

using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Logging
{
    /// <summary>Provides options to customize the messages logged by
    /// <see cref="Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, Microsoft.Extensions.Logging.LogLevel, string)"/>,
    /// <see cref="Extensions.LoggerExtensions.LogCall(Microsoft.Extensions.Logging.ILogger, object?, object, Microsoft.Extensions.Logging.LogLevel, string)"/>,
    /// <see cref="Extensions.LoggerExtensions.LogException(Microsoft.Extensions.Logging.ILogger, Exception, string?, object?[])"/>, and
    /// <see cref="Extensions.LoggerExtensions.LogAllExceptions(Microsoft.Extensions.Logging.ILogger, Exception, string?, object?[])"/>.</summary>
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
        internal string _argumentsDestructureProperty = "Arguments";
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
                _logTemplateWithArguments ??= $"{_callPrefix}{{{_methodNameProperty}}}, {_argumentsPrefix}{{@{_argumentsDestructureProperty}}}";

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

        public static void UseCallPrefix(string callPrefix)
        {
            Instance._callPrefix = callPrefix.WhenNotNullOrEmpty(nameof(callPrefix));

            Instance._logTemplateWithNoArguments = null;
            Instance._logTemplateWithArguments = null;
        }

        public static void UseExceptionPrefix(string exceptionPrefix)
        {
            Instance._exceptionPrefix = exceptionPrefix.WhenNotNullOrEmpty(nameof(exceptionPrefix));

            Instance._logExceptionTemplate = null;
        }

        public static void UseMethodNameProperty(string methodNameProperty)
        {
            Instance._methodNameProperty = methodNameProperty.WhenNotNullOrEmpty(nameof(methodNameProperty));

            Instance._logTemplateWithNoArguments = null;
            Instance._logTemplateWithArguments = null;
        }

        public static void UseExceptionMessageProperty(string exceptionMessageProperty)
        {
            Instance._exceptionMessageProperty = exceptionMessageProperty.WhenNotNullOrEmpty(nameof(exceptionMessageProperty));

            Instance._logExceptionTemplate = null;
        }

        public static void UseArgumentsPrefix(string argumentsPrefix)
        {
            Instance._argumentsPrefix = argumentsPrefix.WhenNotNullOrEmpty(nameof(argumentsPrefix));

            Instance._logTemplateWithArguments = null;
        }

        public static void UseArgumentsDestructureProperty(string argumentsProperty)
        {
            Instance._argumentsDestructureProperty = argumentsProperty.WhenNotNullOrEmpty(nameof(argumentsProperty));

            Instance._logTemplateWithArguments = null;
        }

        public static void IncludeCallerNamespace(bool includeCallerNamespace)
        {
            Instance._includeCallerNamespace = includeCallerNamespace;
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
