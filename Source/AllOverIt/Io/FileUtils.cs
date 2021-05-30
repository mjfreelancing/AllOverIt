using AllOverIt.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AllOverIt.Io
{
    public static class FileUtils
    {
        // Ensures a filename is unique by generating a unique filename if the specified file already exists.
        // If the provided filename exists this method generates a new name that contains a numerical suffix (starting at 1)
        // while ensuring the new filename also does not exist. This method does not create the file so applications should
        // take extra measures to ensure the returned filename is still unique before using it (the file may have since been created
        // by another process.
        [ExcludeFromCodeCoverage]
        public static string CreateUniqueFilename(string baseFilename)
        {
            _ = baseFilename.WhenNotNullOrEmpty(nameof(baseFilename));

            if (!File.Exists(baseFilename))
            {
                return baseFilename;
            }

            string destFileName;
            var counter = 1;
            var dirPart = Path.GetDirectoryName(baseFilename);
            var filePart = Path.GetFileNameWithoutExtension(baseFilename);
            var extPart = Path.HasExtension(baseFilename) ? Path.GetExtension(baseFilename) : "";   // GetExtension() includes the '.' prefix

            do
            {
                destFileName = Path.Combine(dirPart ?? "", $"{filePart}.{counter++}{extPart}");
            } while (File.Exists(destFileName));

            return destFileName;
        }

        // Determines if a given child path is a subfolder of a given parent.
        public static bool PathIsSubFolder(string parentPath, string childPath)
        {
            _ = parentPath.WhenNotNullOrEmpty(nameof(parentPath));
            _ = childPath.WhenNotNullOrEmpty(nameof(childPath));

            var parent = Path.GetFullPath(parentPath);
            var child = Path.GetFullPath(childPath);

            return child.StartsWith(parent);
        }

        // Creates an absolute path from a source path and a relative path
        public static string GetAbsolutePath(string sourcePath, string relativePathOffset)
        {
            _ = sourcePath.WhenNotNullOrEmpty(nameof(sourcePath));
            _ = relativePathOffset.WhenNotNull(nameof(relativePathOffset));   // can be empty

            var outputDirectory = Path.Combine(sourcePath, relativePathOffset);

            return Path.GetFullPath(outputDirectory);
        }

        // Gets an absolute filename after applying a relative offset. If newFileName is not null then the source filename is replaced.
        public static string GetAbsoluteFileName(string sourceFileName, string relativePathOffset, string newFileName = null)
        {
            _ = sourceFileName.WhenNotNullOrEmpty(nameof(sourceFileName));
            _ = relativePathOffset.WhenNotNullOrEmpty(nameof(relativePathOffset));

            var sourceDirectory = Path.GetDirectoryName(sourceFileName);
            var outputPath = GetAbsolutePath(sourceDirectory, relativePathOffset);

            return Path.Combine(outputPath, newFileName ?? Path.GetFileName(sourceFileName));
        }
    }
}
