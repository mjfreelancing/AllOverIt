using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <summary>Contains configuration details for the AppSync Graphql subscription client.</summary>
    public sealed record SubscriptionClientConfiguration : ISubscriptionClientConfiguration
    {
        private string _realTimeUrl;

        /// <inheritdoc />
        public string Host { get; init; }

        /// <inheritdoc />
        public string RealtimeUrl
        {
            get
            {
                _realTimeUrl ??= $"{Host?.Replace("appsync-api", "appsync-realtime-api")}/graphql";
                return _realTimeUrl;
            }

            init => _realTimeUrl = value;
        }

        /// <inheritdoc />
        public IJsonSerializer Serializer { get; init; }

        /// <inheritdoc />
        public IAppSyncAuthorization DefaultAuthorization { get; init; }

        /// <inheritdoc />
        public SubscriptionClientConnectionOptions ConnectionOptions { get; init; } = new();
    }
}