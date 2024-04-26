using AllOverIt.IO;
using System.Diagnostics;

namespace FileSearchDemo
{
    class Program
    {
        static void Main()
        {
            var rootPath = Environment.SystemDirectory;

            Console.WriteLine($"Finding all files under {rootPath}.");
            Console.WriteLine("Press 'S' to (S)how progress or any other key to time process only...");

            var showOutput = Console.ReadKey(true).Key == ConsoleKey.S;

            var files = FileSearch.GetFiles(Environment.SystemDirectory, "*.*",
              DiskSearchOptions.IncludeSubDirectories | DiskSearchOptions.IgnoreUnauthorizedException);

            var fileCount = 0;
            var stopWatch = Stopwatch.StartNew();

            foreach (var file in files)
            {
                if (showOutput)
                {
                    Console.WriteLine(file.FullName);
                }

                ++fileCount;
            }

            stopWatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"All Over It, {fileCount} files found after {stopWatch.ElapsedMilliseconds}ms.");
            Console.ReadKey();
        }
    }
}
