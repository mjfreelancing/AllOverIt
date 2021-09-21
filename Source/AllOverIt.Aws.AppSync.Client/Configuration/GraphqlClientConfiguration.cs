using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <summary>Contains configuration details for AppSync Graphql query and mutation operations.</summary>
    public sealed record GraphqlClientConfiguration
    {
        /// <summary>The fully-qualified AppSync Graphql endpoint to perform query and mutation operations.</summary>
        public string EndPoint { get; init; }

        /// <summary>The serializer to be used for message processing.</summary>
        /// <remarks>See 'AllOverIt.Serialization.NewtonsoftJson' and 'AllOverIt.Serialization.SystemTextJson' for suitable implementations.</remarks>
        public IJsonSerializer Serializer { get; init; }

        /// <summary>Provides the default authorization mode to use for all requests.</summary>
        public IAppSyncAuthorization DefaultAuthorization { get; init; }
    }
}