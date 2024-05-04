using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using ExternalDependencies;
using Microsoft.Extensions.Logging;

namespace AutoRegistrationDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly ILogger<App> _logger;
        private readonly IRepository _repository;
        public App(IRepository repository, ILogger<App> logger)
        {
            _logger = logger.WhenNotNull(nameof(logger));
            _repository = repository.WhenNotNull(nameof(repository));

            Console.WriteLine();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            foreach (var _ in Enumerable.Range(1, 10))
            {
                // The repository is decorated with a logger - so this will do all of the logging
                _repository.GetRandomName();
            }

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