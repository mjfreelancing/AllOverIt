using AllOverIt.Assertion;
using System;
using System.Runtime.CompilerServices;

namespace AllOverIt.Diagnostics.Breadcrumbs.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IBreadcrumbs"/>.</summary>
    public static class BreadcrumbsExtensions
    {
        /// <summary>Adds a message to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="message">The message to be added.</param>
        public static IBreadcrumbs Add(this IBreadcrumbs breadcrumbs, string message)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = message.WhenNotNullOrEmpty(nameof(message));

            return AddBreadcrumb(breadcrumbs, null, message, null);
        }

        /// <summary>Adds a message and related metadata to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="message">The message to be added.</param>
        /// <param name="metadata">Metadata associated with the message.</param>
        public static IBreadcrumbs Add(this IBreadcrumbs breadcrumbs, string message, object metadata)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = message.WhenNotNullOrEmpty(nameof(message));
            _ = metadata.WhenNotNull(nameof(metadata));

            return AddBreadcrumb(breadcrumbs, null, message, metadata);
        }

        /// <summary>Adds a message to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="caller">The caller instance.</param>
        /// <param name="message">The message to be added.</param>
        /// <param name="callerName">The name of the calling method.</param>
        /// <param name="filePath">The file path of the calling method.</param>
        /// <param name="lineNumber">The line number of the calling method file path.</param>
        public static IBreadcrumbs Add(this IBreadcrumbs breadcrumbs, object caller, string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = caller.WhenNotNull(nameof(caller));
            _ = message.WhenNotNullOrEmpty(nameof(message));

            return AddBreadcrumb(breadcrumbs, caller, message, null, callerName, filePath, lineNumber);
        }

        /// <summary>Adds a message and related metadata to the collection of breadcrumbs.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to add the data too.</param>
        /// <param name="caller">The caller instance.</param>
        /// <param name="message">The message to be added.</param>
        /// <param name="metadata">Metadata associated with the message.</param>
        /// <param name="callerName">The name of the calling method.</param>
        /// <param name="filePath">The file path of the calling method.</param>
        /// <param name="lineNumber">The line number of the calling method file path.</param>
        public static IBreadcrumbs Add(this IBreadcrumbs breadcrumbs, object caller, string message, object metadata,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _ = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _ = caller.WhenNotNull(nameof(caller));
            _ = message.WhenNotNullOrEmpty(nameof(message));
            _ = metadata.WhenNotNull(nameof(metadata));

            return AddBreadcrumb(breadcrumbs, caller, message, metadata, callerName, filePath, lineNumber);
        }

        private static IBreadcrumbs AddBreadcrumb(IBreadcrumbs breadcrumbs, object caller, string message, object metadata,
            string callerName = null, string filePath = null, int lineNumber = 0)
        {
            var fullName = (caller, callerName) switch
            {
                (null, null) => null,
                (null, _) => throw new InvalidOperationException("Cannot have a null caller instance."),
                (_, null) => throw new InvalidOperationException("Cannot have a null caller name."),
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

            return breadcrumbs;
        }
    }
}
