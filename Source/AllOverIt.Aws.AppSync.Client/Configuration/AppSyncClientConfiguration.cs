using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Json.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <inheritdoc cref="IAppSyncClientConfiguration" />
    public sealed record AppSyncClientConfiguration : IAppSyncClientConfiguration
    {
        /// <inheritdoc />
        public string EndPoint { get; init; }

        /// <inheritdoc />
        public IJsonSerializer Serializer { get; init; }

        /// <inheritdoc />
        public IAppSyncAuthorization DefaultAuthorization { get; init; }
    }
}