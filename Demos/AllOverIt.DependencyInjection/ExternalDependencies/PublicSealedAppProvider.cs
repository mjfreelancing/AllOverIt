using System;
using Microsoft.Extensions.Logging;

namespace ExternalDependencies
{
    public sealed class PublicSealedAppProvider : IAppProvider
    {
        public PublicSealedAppProvider(DateTime dateTime, ILogger logger)
        {
            logger.LogInformation($"In {nameof(PublicSealedAppProvider)} Constructor at {dateTime:R}");
        }
    }
}