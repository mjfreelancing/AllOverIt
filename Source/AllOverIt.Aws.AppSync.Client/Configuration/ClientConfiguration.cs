using AllOverIt.Aws.AppSync.Client.Subscription.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    public sealed class ClientConfiguration
    {
        public string EndPoint { get; set; }
        public IJsonSerializer Serializer { get; set; }
        public IAppSyncAuthorization DefaultAuthorization { get; set; }
    }
}