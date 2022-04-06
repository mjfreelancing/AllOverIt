using System;
using Microsoft.Extensions.Logging;

namespace ExternalDependencies
{
    internal sealed class InternalSealedAppProvider : IAppProvider
    {
        public InternalSealedAppProvider(DateTime dateTime, ILogger logger)
        {
            logger.LogInformation($"In {nameof(InternalSealedAppProvider)} Constructor at {dateTime:R}");
        }
    }
}