using AllOverIt.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StartupHookDemo
{
    internal class Program
    {
        static async Task Main()
        {
            if (Environment.GetEnvironmentVariable("DOTNET_STARTUP_HOOKS").IsNullOrEmpty())
            {
                Console.WriteLine("The 'DOTNET_STARTUP_HOOKS' environment variable needs to be set in the launchsettings.json file.");
                Console.WriteLine();
            }

            Console.WriteLine("Demo app is running...Press any key to exit.");

            while (!Console.KeyAvailable)
            {
                _ = Enumerable.Range(100_000, 500_000)
                    .Select(value => $"{value}")
                    .ToArray();

                await Task.Delay(1000);
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}