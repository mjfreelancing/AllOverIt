using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using AllOverIt.Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace ConsoleLoggingDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly IAppService _appService;
        private readonly ILogger<App> _logger;

        public App(IAppService appService, ILogger<App> logger)
        {
            _appService = appService.WhenNotNull();
            _logger = logger.WhenNotNull();

            _logger.LogCall(this);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogCall(this);

            await _appService.GetRandomNumbersAsync(25, cancellationToken);

            try
            {
                throw new ApplicationException("This is an example exception for logging");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception);
            }

            try
            {
                throw new ApplicationException("This is an example exception with a custom template for logging");
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "Error at {Timestamp}: {Exception}", DateTime.Now.ToString("O"), exception.Message);
            }

            ExitCode = 0;
        }

        public override void OnStopping()
        {
            // The logger is not available at this point
            Console.WriteLine("=> App is stopping");
        }

        public override void OnStopped()
        {
            // The logger is not available at this point
            Console.WriteLine("=> App is stopped");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}