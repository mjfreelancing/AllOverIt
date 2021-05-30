using AllOverIt.Io;
using System;

namespace DirectorySearchDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootPath = Environment.SystemDirectory;

            Console.WriteLine($"Finding all directories under {rootPath}, press any key to continue...");
            Console.ReadKey(true);

            var directories = DirectorySearch.GetDirectories(
              rootPath,
              DiskSearchOptions.IncludeSubDirectories | DiskSearchOptions.IgnoreUnauthorizedException);

            foreach (var directory in directories)
            {
                Console.WriteLine(directory.FullName);
            }

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
