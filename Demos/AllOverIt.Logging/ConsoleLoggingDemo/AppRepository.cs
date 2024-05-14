using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace ConsoleLoggingDemo
{
    internal sealed class AppRepository : IAppRepository
    {
        private readonly ILogger<AppRepository> _logger;

        public AppRepository(ILogger<AppRepository> logger)
        {
            _logger = logger.WhenNotNull();

            _logger.LogCall(this);
        }

        public int[] GetRandomNumbers(int count, int maxValue, CancellationToken cancellationToken)
        {
            _logger.LogCall(this, new { Count = count, MaxValue = maxValue, ThreadId = Environment.CurrentManagedThreadId });

            return Enumerable
                .Range(1, count)
                .SelectToArray(_ => Random.Shared.Next(maxValue + 1));
        }
    }
}