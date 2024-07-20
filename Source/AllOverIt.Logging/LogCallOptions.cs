#nullable enable

using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging
{
    /// <summary>Provides options to customize the messages logged by
    /// <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>,
    /// <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>,
    /// <see cref="Extensions.LoggerExtensions.LogException(ILogger, Exception, string?, object?[])"/>, and
    /// <see cref="Extensions.LoggerExtensions.LogAllExceptions(ILogger, Exception, string?, object?[])"/>.</summary>
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

        /// <summary>Sets the log prefix to use when calling <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>
        /// or <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>. The default log templates are
        /// <c>"Call: {MethodName}"</c> and <c>"Call: {MethodName}, Arguments = {@Arguments}"</c>, respectively. This method will replace the
        /// <c>"Call: "</c> portion.
        /// </summary>
        /// <param name="callPrefix">The call prefix to use.</param>
        public static void UseCallPrefix(string callPrefix)
        {
            Instance._callPrefix = callPrefix.WhenNotNullOrEmpty();

            Instance._logTemplateWithNoArguments = null;
            Instance._logTemplateWithArguments = null;
        }

        /// <summary>Sets the exception prefix to use when calling <see cref="Extensions.LoggerExtensions.LogException(ILogger, Exception, string?, object?[])"/>.
        /// The default log template is <c>"Error: {ErrorMessage}"</c>. This method will replace the <c>"Error: "</c> portion.</summary>
        /// <param name="exceptionPrefix">The exception prefix to use.</param>
        public static void UseExceptionPrefix(string exceptionPrefix)
        {
            Instance._exceptionPrefix = exceptionPrefix.WhenNotNullOrEmpty();

            Instance._logExceptionTemplate = null;
        }

        /// <summary>Sets the method name property to use when calling <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>
        /// or <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>. The default log templates are
        /// <c>"Call: {MethodName}"</c> and <c>"Call: {MethodName}, Arguments = {@Arguments}"</c>, respectively. This method will replace the
        /// <c>"MethodName"</c> portion.
        /// </summary>
        /// <param name="methodNameProperty">The method name property to use.</param>
        public static void UseMethodNameProperty(string methodNameProperty)
        {
            Instance._methodNameProperty = methodNameProperty.WhenNotNullOrEmpty();

            Instance._logTemplateWithNoArguments = null;
            Instance._logTemplateWithArguments = null;
        }

        /// <summary>Sets the exception message property to use when calling <see cref="Extensions.LoggerExtensions.LogException(ILogger, Exception, string?, object?[])"/>.
        /// The default log template is <c>"Error: {ErrorMessage}"</c>. This method will replace the <c>"ErrorMessage"</c> portion.</summary>
        /// <param name="exceptionMessageProperty">The exception message property to use.</param>
        public static void UseExceptionMessageProperty(string exceptionMessageProperty)
        {
            Instance._exceptionMessageProperty = exceptionMessageProperty.WhenNotNullOrEmpty();

            Instance._logExceptionTemplate = null;
        }

        /// <summary>Sets the arguments prefix to use when calling <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>.
        /// The default log template is <c>"Call: {MethodName}, Arguments = {@Arguments}"</c>. This method will replace the <c>"Arguments = "</c> portion.</summary>
        /// <param name="argumentsPrefix">The arguments prefix to use.</param>
        public static void UseArgumentsPrefix(string argumentsPrefix)
        {
            Instance._argumentsPrefix = argumentsPrefix.WhenNotNullOrEmpty();

            Instance._logTemplateWithArguments = null;
        }

        /// <summary>Sets the arguments destructure property to use when calling <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>.
        /// The default log template is <c>"Call: {MethodName}, Arguments = {@Arguments}"</c>. This method will replace the <c>"Arguments"</c> portion within <c>"{@Arguments}"</c>.</summary>
        /// <param name="argumentsProperty">The arguments property to use.</param>
        public static void UseArgumentsDestructureProperty(string argumentsProperty)
        {
            Instance._argumentsDestructureProperty = argumentsProperty.WhenNotNullOrEmpty();

            Instance._logTemplateWithArguments = null;
        }

        /// <summary>For non-static methods, when calling <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, LogLevel, string)"/>
        /// or <see cref="Extensions.LoggerExtensions.LogCall(ILogger, object?, object, LogLevel, string)"/>, this method determines if the caller's
        /// class name is fully qualified with its namespace.</summary>
        /// <param name="includeCallerNamespace"><see langword="True"/> to include the namespace and class name, otherwise <see langword="False"/> to only include the class name.</param>
        public static void IncludeCallerNamespace(bool includeCallerNamespace)
        {
            Instance._includeCallerNamespace = includeCallerNamespace;
        }

        internal static string GetCallerFullName(object? caller, string callerName)
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
