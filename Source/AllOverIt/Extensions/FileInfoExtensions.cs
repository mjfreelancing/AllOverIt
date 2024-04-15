using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace AllOverIt.Extensions
{
    /// <summary>Contains extension methods for use with <see cref="FileInfo"/> instances.</summary>
    [ExcludeFromCodeCoverage]
    public static class FileInfoExtensions
    {
        /// <summary>Gets the fully qualified path of the directory or file.</summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> instance containing information about a file or directory.</param>
        /// <returns>Returns the fully qualified path of the directory or file.</returns>
        /// <remarks>This method returns the equivalent of <c>fileInfo.FullName</c> with the exception that if the length of the string is 260 or
        /// more characters the method falls back to using reflection to obtain the string rather than throw a <c>PathTooLongException</c> exception.</remarks>
        public static string GetFullName(this FileInfo fileInfo)
        {
            try
            {
                return fileInfo.FullName;
            }
            catch (PathTooLongException)
            {
                return (string) fileInfo.GetType()
                  .GetField("FullPath", BindingFlags.Instance | BindingFlags.NonPublic)!
                  .GetValue(fileInfo)!;
            }
        }

        /// <summary>Determines whether the <paramref name="fileInfo"/> instance represents a directory.</summary>
        /// <param name="fileInfo">The <c>FileInfo</c> instance containing information about a file or directory.</param>
        /// <returns>Returns <c>true</c> if the <paramref name="fileInfo"/> instance represents a directory.</returns>
        public static bool IsDirectory(this FileInfo fileInfo)
        {
            return (fileInfo.Attributes & FileAttributes.Directory) != 0;
        }
    }
}
