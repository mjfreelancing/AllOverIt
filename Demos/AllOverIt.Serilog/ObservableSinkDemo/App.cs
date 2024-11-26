using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using AllOverIt.Serilog.Sinks.Observable;
using Microsoft.Extensions.Logging;

namespace ObservableSinkDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly IObservableSink _observableSink;
        private readonly ILogger<App> _logger;

        public App(IObservableSink observableSink, ILogger<App> logger)
        {
            _observableSink = observableSink.WhenNotNull();
            _logger = logger.WhenNotNull();

            Console.WriteLine();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            var subscription = _observableSink.Subscribe(logEvent =>
            {
                Console.WriteLine($"Received: {logEvent.RenderMessage()} at {logEvent.Timestamp}");
            });

            using (subscription)
            {
                // Demo logging to multiple threads is captured by the circular buffer
                var tasks = Enumerable
                    .Range(100, 50)
                    .Select(value => Task.Run(() => _logger.LogInformation("{Value}", value)));

                await Task.WhenAll(tasks);
            }

            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.WriteLine();

            Console.ReadKey();
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