using AllOverIt.Assertion;
using System.Runtime.CompilerServices;

namespace AllOverIt.Diagnostics.Breadcrumbs.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IBreadcrumbs"/>.</summary>
    public static class BreadcrumbsExtensions
    {
        /// <summary>Adds a message that includes the name of the calling object and method.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="caller">The caller instance.</param>
        /// <param name="metadata">Metadata associated with the message.</param>
        /// <param name="callerName">The name of the calling method.</param>
        public static void AddCallSite(this IBreadcrumbs breadcrumbs, object caller, object? metadata = null,
            [CallerMemberName] string callerName = "")
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = caller.WhenNotNull(nameof(caller));
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));      // Pointless calling this method without a caller name

            var message = $"Call Site: {caller.GetType().FullName}.{callerName}()";

            AddBreadcrumb(breadcrumbs, caller, message, metadata, callerName);
        }

        /// <summary>Adds a message that includes the name of the calling object and method.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="caller">The caller instance.</param>
        /// <param name="metadata">Metadata associated with the message.</param>
        /// <param name="callerName">The name of the calling method.</param>
        /// <param name="filePath">The file path of the calling method.</param>
        /// <param name="lineNumber">The line number of the calling method file path.</param>
        public static void AddExtendedCallSite(this IBreadcrumbs breadcrumbs, object caller, object? metadata = null,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = caller.WhenNotNull(nameof(caller));
            _ = callerName.WhenNotNullOrEmpty(nameof(callerName));      // Pointless calling this method without a caller name
            _ = filePath.WhenNotNullOrEmpty(nameof(filePath));          // Pointless calling this method without a file path

            var message = $"Call Site: {caller.GetType().FullName}.{callerName}(), at {filePath}:{lineNumber}";

            AddBreadcrumb(breadcrumbs, caller, message, metadata, callerName, filePath, lineNumber);
        }

        /// <summary>Adds a message to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="message">The message to be added.</param>
        public static void Add(this IBreadcrumbs breadcrumbs, string message)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = message.WhenNotNullOrEmpty(nameof(message));

            AddBreadcrumb(breadcrumbs, null, message, null, null);
        }

        /// <summary>Adds a message and related metadata to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="message">The message to be added.</param>
        /// <param name="metadata">Metadata associated with the message.</param>
        public static void Add(this IBreadcrumbs breadcrumbs, string message, object metadata)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = message.WhenNotNullOrEmpty(nameof(message));
            _ = metadata.WhenNotNull(nameof(metadata));

            AddBreadcrumb(breadcrumbs, null, message, metadata, null);
        }

        /// <summary>Adds a message to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="caller">The caller instance.</param>
        /// <param name="message">The message to be added.</param>
        /// <param name="callerName">The name of the calling method.</param>
        /// <param name="filePath">The file path of the calling method.</param>
        /// <param name="lineNumber">The line number of the calling method file path.</param>
        public static void Add(this IBreadcrumbs breadcrumbs, object caller, string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = caller.WhenNotNull(nameof(caller));
            _ = message.WhenNotNullOrEmpty(nameof(message));

            AddBreadcrumb(breadcrumbs, caller, message, null, callerName, filePath, lineNumber);
        }

        /// <summary>Adds a message and related metadata to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="caller">The caller instance.</param>
        /// <param name="message">The message to be added.</param>
        /// <param name="metadata">Metadata associated with the message.</param>
        /// <param name="callerName">The name of the calling method.</param>
        /// <param name="filePath">The file path of the calling method.</param>
        /// <param name="lineNumber">The line number of the calling method file path.</param>
        public static void Add(this IBreadcrumbs breadcrumbs, object caller, string message, object metadata,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = caller.WhenNotNull(nameof(caller));
            _ = message.WhenNotNullOrEmpty(nameof(message));
            _ = metadata.WhenNotNull(nameof(metadata));

            AddBreadcrumb(breadcrumbs, caller, message, metadata, callerName, filePath, lineNumber);
        }

        internal static void AddBreadcrumb(IBreadcrumbs breadcrumbs, object? caller, string? message, object? metadata,
            string? callerName, string? filePath = null, int lineNumber = 0)
        {
            var fullName = (caller, callerName) switch
            {
                (null, null) => null,
                (null, _) => throw new InvalidOperationException("Cannot have a null caller instance."),        // Cannot test this if the method is private
                (_, null) => throw new InvalidOperationException("Cannot have a null caller name."),            // Cannot test this if the method is private
                (_, "") => $"{caller.GetType().FullName}",
                (_, _) => $"{caller.GetType().FullName}.{callerName}"
            };

            var breadcrumb = new BreadcrumbData
            {
                CallerName = fullName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Message = message,
                Metadata = metadata
            };

            breadcrumbs.Add(breadcrumb);
        }
    }
}
