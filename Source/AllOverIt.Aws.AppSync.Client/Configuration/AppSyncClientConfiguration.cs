using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Json.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <inheritdoc cref="IAppSyncClientConfiguration" />
    public sealed class AppSyncClientConfiguration : IAppSyncClientConfiguration
    {
        /// <inheritdoc />
        public required string EndPoint { get; init; }

        /// <inheritdoc />
        public required IJsonSerializer Serializer { get; init; }

        /// <inheritdoc />
        public required IAppSyncAuthorization DefaultAuthorization { get; init; }
    }
}