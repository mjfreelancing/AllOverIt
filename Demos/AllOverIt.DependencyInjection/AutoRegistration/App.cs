using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using ExternalDependencies;
using Microsoft.Extensions.Logging;

namespace AutoRegistration
{
    public sealed class App : ConsoleAppBase
    {
        private readonly ILogger<App> _logger;

        public App(ILogger<App> logger, IEnumerable<IAppProvider> consoleLoggers)
        {
            _logger = logger.WhenNotNull(nameof(logger));

            foreach (var consoleLogger in consoleLoggers)
            {
                Console.WriteLine($"{consoleLogger.GetType().GetFriendlyName()} has been injected into {nameof(App)}");
            }

            Console.WriteLine();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.WriteLine();
            Console.ReadKey();

            return Task.CompletedTask;
        }

        public override void OnStopping()
        {
            _logger.LogInformation("App is stopping");
        }

        public override void OnStopped()
        {
            _logger.LogInformation("App is stopped");
        }
    }
}