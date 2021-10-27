using AllOverIt.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AllOverIt.Io
{
    /// <summary>Contains file and path related utilities.</summary>
    public static class FileUtils
    {
        /// <summary>Ensures a filename is unique by generating a unique filename if the specified file already exists.</summary>
        /// <param name="filename">The filename to check for uniqueness.</param>
        /// <returns>A filename that will not conflict with an existing file.</returns>
        /// <remarks>If the provided filename exists this method generates a new name that contains a numerical suffix (starting at 1) while
        /// ensuring the new filename also does not exist. This method does not create the file so applications should take extra measures to
        /// ensure the returned filename is still unique before using it (the file may have since been created by another process).</remarks>
        [ExcludeFromCodeCoverage]
        public static string CreateUniqueFilename(string filename)
        {
            _ = filename.WhenNotNullOrEmpty(nameof(filename));

            if (!File.Exists(filename))
            {
                return filename;
            }

            string destFileName;
            var counter = 1;
            var dirPart = Path.GetDirectoryName(filename);
            var filePart = Path.GetFileNameWithoutExtension(filename);
            var extPart = Path.HasExtension(filename) ? Path.GetExtension(filename) : "";   // GetExtension() includes the '.' prefix

            do
            {
                destFileName = Path.Combine(dirPart ?? "", $"{filePart}.{counter++}{extPart}");
            } while (File.Exists(destFileName));

            return destFileName;
        }

        /// <summary>Determines if the provided child path is a subfolder of the provided parent path.</summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="childPath">The child path.</param>
        /// <returns>True if the child path is an immediate subfolder of the parent path.</returns>
        public static bool PathIsSubFolder(string parentPath, string childPath)
        {
            _ = parentPath.WhenNotNullOrEmpty(nameof(parentPath));
            _ = childPath.WhenNotNullOrEmpty(nameof(childPath));

            var parent = Path.GetFullPath(parentPath);
            var child = Path.GetFullPath(childPath);

            return child.StartsWith(parent);
        }

        /// <summary>Creates an absolute path from a source path and a relative path.</summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="relativePath">The relative path to apply to the source path.</param>
        /// <returns>The absolute path derived from combining the source and relative paths.</returns>
        public static string GetAbsolutePath(string sourcePath, string relativePath)
        {
            _ = sourcePath.WhenNotNullOrEmpty(nameof(sourcePath));
            _ = relativePath.WhenNotNull(nameof(relativePath));   // can be empty

            var outputDirectory = Path.Combine(sourcePath, relativePath);

            return Path.GetFullPath(outputDirectory);
        }

        /// <summary>Gets an absolute filename after applying a relative path to the original source filename.</summary>
        /// <param name="sourceFileName">The original source filename (with path).</param>
        /// <param name="relativePath">The relative path to apply to the path portion of the source filename.</param>
        /// <param name="newFileName">If not null then the source filename is replaced.</param>
        /// <returns>The absolute filename derived from applying a relative path to the original source filename.</returns>
        public static string GetAbsoluteFileName(string sourceFileName, string relativePath, string newFileName = null)
        {
            _ = sourceFileName.WhenNotNullOrEmpty(nameof(sourceFileName));
            _ = relativePath.WhenNotNullOrEmpty(nameof(relativePath));

            var sourceDirectory = Path.GetDirectoryName(sourceFileName);
            var outputPath = GetAbsolutePath(sourceDirectory, relativePath);

            return Path.Combine(outputPath, newFileName ?? Path.GetFileName(sourceFileName));
        }
    }
}
