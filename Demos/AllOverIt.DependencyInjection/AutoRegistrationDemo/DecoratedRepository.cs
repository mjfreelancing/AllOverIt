using AllOverIt.Logging.Extensions;
using ExternalDependencies;
using Microsoft.Extensions.Logging;

namespace AutoRegistrationDemo
{
    internal sealed class DecoratedRepository : IRepository
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public DecoratedRepository(IRepository repository, ILogger<DecoratedRepository> logger)
        {
            _repository = repository;
            _logger = logger;

            logger.LogCall(null);
        }

        public string GetRandomName()
        {
            _logger.LogCall(this);

            var name = _repository.GetRandomName();

            _logger.LogInformation("Returning the name '{Name}'", name);

            return name;
        }
    }
}