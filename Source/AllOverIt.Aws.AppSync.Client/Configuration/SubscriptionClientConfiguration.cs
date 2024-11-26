using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Json.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    /// <inheritdoc cref="ISubscriptionClientConfiguration" />
    public sealed class SubscriptionClientConfiguration : ISubscriptionClientConfiguration
    {
        private string? _realTimeUrl;

        /// <inheritdoc />
        public required string Host { get; init; }

        /// <inheritdoc />
        public string? RealtimeUrl
        {
            get
            {
                _realTimeUrl ??= $"{Host?.Replace("appsync-api", "appsync-realtime-api")}/graphql";
                return _realTimeUrl;
            }

            init => _realTimeUrl = value;
        }

        /// <inheritdoc />
        public required IJsonSerializer Serializer { get; init; }

        /// <inheritdoc />
        public required IAppSyncAuthorization DefaultAuthorization { get; init; }

        /// <inheritdoc />
        public SubscriptionClientConnectionOptions ConnectionOptions { get; init; } = new();
    }
}