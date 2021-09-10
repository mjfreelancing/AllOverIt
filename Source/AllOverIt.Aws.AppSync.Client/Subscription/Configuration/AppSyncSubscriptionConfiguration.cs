using AllOverIt.Aws.AppSync.Client.Subscription.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Configuration
{
    public sealed class AppSyncSubscriptionConfiguration
    {
        private string _realTimeUrl;

        public string HostUrl { get; set; }

        // If not set, this Url is derived from the HostUrl by replacing 'appsync-api' with 'appsync-realtime-api'.
        public string RealTimeUrl
        {
            get
            {
                _realTimeUrl ??= HostUrl?.Replace("appsync-api", "appsync-realtime-api");
                return _realTimeUrl;
            }

            set => _realTimeUrl = value;
        }

        public IAppSyncAuthorization DefaultAuthorization { get; set; }
        public IJsonSerializer Serializer { get; set; }
        public AppSyncClientConnectionOptions ConnectionOptions { get; } = new();
    }
}