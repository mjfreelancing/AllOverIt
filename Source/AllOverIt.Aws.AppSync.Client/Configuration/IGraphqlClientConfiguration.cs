using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <summary>Represents configuration details for AppSync Graphql query and mutation operations.</summary>
    public interface IGraphqlClientConfiguration
    {
        /// <summary>The fully-qualified AppSync Graphql endpoint to perform query and mutation operations.</summary>
        string EndPoint { get; }

        /// <summary>The serializer to be used for message processing.</summary>
        /// <remarks>See 'AllOverIt.Serialization.NewtonsoftJson' and 'AllOverIt.Serialization.SystemTextJson' for suitable implementations.</remarks>
        IJsonSerializer Serializer { get; }

        /// <summary>Provides the default authorization mode to use for all requests.</summary>
        IAppSyncAuthorization DefaultAuthorization { get; }
    }
}