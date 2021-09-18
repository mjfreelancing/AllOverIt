using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    public sealed record SubscriptionClientConfiguration
    {
        private string _realTimeUrl;

        // AppSync's graphql host Url without https:// or /graphql suffix
        public string Host { get; init; }

        // If not set, this Url is derived from the Host by replacing 'appsync-api' with 'appsync-realtime-api' and appending '/graphql'.
        // If provided it must be the full custom realtime Url, but without https://
        public string RealTimeUrl
        {
            get
            {
                _realTimeUrl ??= $"{Host?.Replace("appsync-api", "appsync-realtime-api")}/graphql";
                return _realTimeUrl;
            }

            init => _realTimeUrl = value;
        }

        public IAppSyncAuthorization DefaultAuthorization { get; init; }
        public IJsonSerializer Serializer { get; init; }
        public SubscriptionClientConnectionOptions ConnectionOptions { get; } = new();
    }
}