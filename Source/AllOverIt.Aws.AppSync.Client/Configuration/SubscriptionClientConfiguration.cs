using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Json.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <inheritdoc cref="ISubscriptionClientConfiguration" />
    public sealed class SubscriptionClientConfiguration : ISubscriptionClientConfiguration
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