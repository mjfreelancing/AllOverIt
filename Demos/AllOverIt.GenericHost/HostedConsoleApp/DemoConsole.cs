using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostedConsoleApp
{
    public sealed class DemoConsole : ConsoleAppBase
    {
        private readonly ILogger<DemoConsole> _logger;

        public DemoConsole(ILogger<DemoConsole> logger)
        {
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("StartAsync");

            // providing an initial delay so the background worker can show it is alive
            await Task.Delay(3000, cancellationToken);

            Console.WriteLine();
            Console.WriteLine("ENVIRONMENT VARIABLES");
            Console.WriteLine("=====================");

            var variables = Environment.GetEnvironmentVariables().ToSerializedDictionary();

            foreach (var (key, value) in variables)
            {
                Console.WriteLine($"{key} = {value}");
            }

            Console.WriteLine();

            // providing another delay so the background worker can show it is still alive
            await Task.Delay(3000, cancellationToken);

            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It (the background worker will continue until a key is pressed).");
            Console.WriteLine();
            Console.ReadKey();
        }

        public override void OnStopping()
        {
            _logger.LogInformation("DemoConsole is stopping");
        }

        public override void OnStopped()
        {
            _logger.LogInformation("DemoConsole is stopped");
        }
    }
}