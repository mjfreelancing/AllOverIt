using ExternalDependencies;
using Microsoft.Extensions.Logging;

namespace AutoRegistration
{
    internal sealed class DecoratedRepository : IRepository
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public DecoratedRepository(IRepository repository, ILogger<DecoratedRepository> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public string GetRandomName()
        {
            _logger.LogInformation("Inside DecoratedRepository");

            var name = _repository.GetRandomName();

            _logger.LogInformation($"Returning the name '{name}");

            return name;
        }
    }
}