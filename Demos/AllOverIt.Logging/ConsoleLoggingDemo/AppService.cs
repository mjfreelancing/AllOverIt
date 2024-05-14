using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace ConsoleLoggingDemo
{
    internal sealed class AppService : IAppService
    {
        private readonly IAppRepository _appRepository;
        private readonly ILogger<AppService> _logger;

        public AppService(IAppRepository appRepository, ILogger<AppService> logger)
        {
            _appRepository = appRepository.WhenNotNull();
            _logger = logger.WhenNotNull();

            _logger.LogCall(this);
        }

        public async Task<int[]> GetRandomNumbersAsync(int count, CancellationToken cancellationToken)
        {
            _logger.LogCall(this, new { count });

            var numbers = await Enumerable
                .Range(1, count)
                .SelectAsParallelAsync(async (number, token) =>
                {
                    await Task.Delay(10, token);

                    var numbers = _appRepository.GetRandomNumbers(100, Random.Shared.Next((number * 100) + 1), token);

                    return numbers;
                }, Environment.ProcessorCount, cancellationToken)
                .ToListAsync(cancellationToken);

            return numbers.SelectManyToArray(numbers => numbers);
        }
    }
}