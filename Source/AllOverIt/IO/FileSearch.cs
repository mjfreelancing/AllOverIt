using AllOverIt.Assertion;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.IO
{
    /// <summary>Contains search related operations for files that support cancellation.</summary>
    [ExcludeFromCodeCoverage]
    public static class FileSearch
    {
        /// <summary>Gets an enumerable collection of files matching a file specification in the current directory.</summary>
        /// <param name="path">The relative or absolute path of the directory to search.</param>
        /// <param name="filter">The file specification to filter the returned results.</param>
        /// <param name="searchOptions">Specifies options for the search operation.</param>
        /// <returns>Returns an enumerable collection of files.</returns>
        public static IEnumerable<FileInfo> GetFiles(string path, string filter, DiskSearchOptions searchOptions)
        {
            return DoGetFiles(new DirectoryInfo(path), filter, searchOptions, CancellationToken.None);
        }

        /// <summary>Gets an enumerable collection of files matching a file specification in the current directory. Enumeration
        /// can be interrupted by cancelling the provided <c>CancellationToken</c>.</summary>
        /// <param name="path">The relative or absolute path of the directory to search.</param>
        /// <param name="filter">The file specification to filter the returned results.</param>
        /// <param name="searchOptions">Specifies options for the search operation.</param>
        /// <param name="cancellationToken">The cancellation token to signal when enumeration needs to be prematurely completed.</param>
        /// <returns>Returns an enumerable collection of files. The enumeration may not be complete if the <c>cancellationToken</c>
        /// is cancelled.</returns>
        public static IEnumerable<FileInfo> GetFiles(string path, string filter, DiskSearchOptions searchOptions,
          CancellationToken cancellationToken)
        {
            return DoGetFiles(new DirectoryInfo(path), filter, searchOptions, cancellationToken);
        }

        private static IEnumerable<FileInfo> DoGetFiles(DirectoryInfo directoryInfo, string filter, DiskSearchOptions searchOptions,
          CancellationToken cancellationToken)
        {
            // NOTE: Cannot implement using SearchOption.AllDirectories / SearchOption.TopDirectoryOnly as a non-recursive
            //       alternative because 'return yield' cannot be used within a try block.

            _ = directoryInfo.WhenNotNull(nameof(directoryInfo));
            _ = filter.WhenNotNullOrEmpty(nameof(filter));

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            // ignore ReparsePoints to avoid infinite recursion
            if (FileAttributes.ReparsePoint == (File.GetAttributes(directoryInfo.FullName) & FileAttributes.ReparsePoint))
            {
                yield break;
            }

            IEnumerable<FileInfo>? files = null;

            try
            {
                files = directoryInfo.EnumerateFiles(filter);
            }

            catch (UnauthorizedAccessException)
            {
                if (!searchOptions.HasFlag(DiskSearchOptions.IgnoreUnauthorizedException))
                {
                    throw;
                }
            }

            catch (IOException)
            {
                // file in use
                if (!searchOptions.HasFlag(DiskSearchOptions.IgnoreIoException))
                {
                    throw;
                }
            }

            // files will be null if exceptions are ignored
            foreach (var file in files ?? [])
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                yield return file;
            }

            if (searchOptions.HasFlag(DiskSearchOptions.IncludeSubDirectories))
            {
                var nonRecursiveSearchOptions = searchOptions & ~DiskSearchOptions.IncludeSubDirectories;

                // the loops below will yield early if the token is cancelled
                var directories = DirectorySearch.GetDirectories(directoryInfo.FullName, nonRecursiveSearchOptions, cancellationToken);

                foreach (var subdirectory in directories)
                {
                    var subFiles = DoGetFiles(subdirectory, filter, searchOptions, cancellationToken);

                    foreach (var subFile in subFiles)
                    {
                        yield return subFile;
                    }
                }
            }
        }
    }
}
