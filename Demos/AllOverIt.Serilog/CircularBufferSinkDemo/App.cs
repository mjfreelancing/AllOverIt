using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using Microsoft.Extensions.Logging;

namespace CircularBufferSinkDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly ICircularBufferSinkMessages _sinkMessages;
        private readonly ILogger<App> _logger;

        public App(ICircularBufferSinkMessages sinkMessages, ILogger<App> logger)
        {
            _sinkMessages = sinkMessages.WhenNotNull(nameof(sinkMessages));
            _logger = logger.WhenNotNull(nameof(logger));

            Console.WriteLine();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            // Demo logging to multiple threads is captured by the circular buffer
            var tasks = Enumerable
                .Range(100, 50)
                .Select(value => Task.Run(() => _logger.LogInformation("{Value}", value)));

            await Task.WhenAll(tasks);

            Console.WriteLine();
            Console.WriteLine($"Items in circular buffer (capacity={_sinkMessages.Capacity}) =>");
            Console.WriteLine();

            foreach (var sinkItem in _sinkMessages)
            {
                Console.WriteLine(sinkItem.FormattedMessage);
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