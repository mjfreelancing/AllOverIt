using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <summary>Contains configuration details for the AppSync Graphql subscription client.</summary>
    public sealed record SubscriptionClientConfiguration
    {
        private string _realTimeUrl;

        /// <summary>AppSync's graphql host Url without the https:// prefix or /graphql suffix.</summary>
        public string Host { get; init; }

        /// <summary>The realtime url used for subscriptions without the https:// prefix.</summary>
        /// <remarks>If not set, this Url is derived from the Host by replacing 'appsync-api' with 'appsync-realtime-api' and
        /// appending '/graphql'. If a value is provided then it must be the full custom domain realtime Url without the
        /// https:// prefix.</remarks>
        public string RealTimeUrl
        {
            get
            {
                _realTimeUrl ??= $"{Host?.Replace("appsync-api", "appsync-realtime-api")}/graphql";
                return _realTimeUrl;
            }

            init => _realTimeUrl = value;
        }
        
        /// <summary>The serializer to be used for message processing.</summary>
        /// <remarks>See 'AllOverIt.Serialization.NewtonsoftJson' and 'AllOverIt.Serialization.SystemTextJson' for suitable implementations.</remarks>
        public IJsonSerializer Serializer { get; init; }

        /// <summary>Provides the default authorization mode to use for all requests.</summary>
        public IAppSyncAuthorization DefaultAuthorization { get; init; }

        /// <summary>Provides subscription connection options.</summary>
        public SubscriptionClientConnectionOptions ConnectionOptions { get; } = new();
    }
}