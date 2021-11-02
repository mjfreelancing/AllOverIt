using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;

namespace AllOverIt.Io
{
    /// <summary>Contains search related operations for directories that support cancellation.</summary>
    [ExcludeFromCodeCoverage]
    public static class DirectorySearch
    {
        /// <summary>Gets an enumerable collection of directories in the current directory.</summary>
        /// <param name="path">The relative or absolute path of the directory to search.</param>
        /// <param name="searchOptions">Specifies options for the search operation.</param>
        /// <returns>Returns an enumerable collection of directories.</returns>
        public static IEnumerable<DirectoryInfo> GetDirectories(string path, DiskSearchOptions searchOptions)
        {
            return DoGetDirectories(new DirectoryInfo(path), searchOptions, CancellationToken.None);
        }

        /// <summary>Gets an enumerable collection of directories in the current directory. Enumeration
        /// can be interrupted by cancelling the provided <c>CancellationToken</c>.</summary>
        /// <param name="path">The relative or absolute path of the directory to search.</param>
        /// <param name="searchOptions">Specifies options for the search operation.</param>
        /// <param name="cancellationToken">The cancellation token to signal when enumeration needs to be prematurely completed.</param>
        /// <returns>Returns an enumerable collection of directories. The enumeration may not be complete if the <c>cancellationToken</c>
        /// is cancelled.</returns>
        public static IEnumerable<DirectoryInfo> GetDirectories(string path, DiskSearchOptions searchOptions,
          CancellationToken cancellationToken)
        {
            return DoGetDirectories(new DirectoryInfo(path), searchOptions, cancellationToken);
        }

        private static IEnumerable<DirectoryInfo> DoGetDirectories(DirectoryInfo directoryInfo, DiskSearchOptions searchOptions,
          CancellationToken cancellationToken)
        {
            // NOTE: Cannot implement using SearchOption.AllDirectories / SearchOption.TopDirectoryOnly as a non-recursive
            //       alternative because 'return yield' cannot be used within a try block.

            _ = directoryInfo.WhenNotNull(nameof(directoryInfo));

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            IEnumerable<DirectoryInfo> directories = null;

            try
            {
                directories = directoryInfo.EnumerateDirectories();
            }

            catch (UnauthorizedAccessException)
            {
                if (!searchOptions.HasFlag(DiskSearchOptions.IgnoreUnauthorizedException))
                {
                    throw;
                }
            }

            foreach (var directory in directories ?? Enumerable.Empty<DirectoryInfo>())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                yield return directory;

                if (searchOptions.HasFlag(DiskSearchOptions.IncludeSubDirectories))
                {
                    var subDirectories = DoGetDirectories(directory, searchOptions, cancellationToken);

                    foreach (var subDirectory in subDirectories)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield break;
                        }

                        yield return subDirectory;
                    }
                }
            }
        }
    }
}
