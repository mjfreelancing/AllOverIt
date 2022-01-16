using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <summary>Contains configuration details for AppSync Graphql query and mutation operations.</summary>
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