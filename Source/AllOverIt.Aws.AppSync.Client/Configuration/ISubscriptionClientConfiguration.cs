using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <summary>Represents configuration details for the AppSync Graphql subscription client.</summary>
    public interface ISubscriptionClientConfiguration
    {
        /// <summary>AppSync's graphql host Url without the https:// prefix or /graphql suffix.</summary>
        string Host { get; }

        /// <summary>The realtime url used for subscriptions without the https:// prefix.</summary>
        /// <remarks>If not set, this Url is derived from the Host by replacing 'appsync-api' with 'appsync-realtime-api' and
        /// appending '/graphql'. If a value is provided then it must be the full custom domain realtime Url without the
        /// https:// prefix.</remarks>
        string RealTimeUrl { get; }

        /// <summary>The serializer to be used for message processing.</summary>
        /// <remarks>See 'AllOverIt.Serialization.NewtonsoftJson' and 'AllOverIt.Serialization.SystemTextJson' for suitable implementations.</remarks>
        IJsonSerializer Serializer { get; }

        /// <summary>Provides the default authorization mode to use for all requests.</summary>
        IAppSyncAuthorization DefaultAuthorization { get; }

        /// <summary>Provides subscription connection options.</summary>
        SubscriptionClientConnectionOptions ConnectionOptions { get; }
    }
}