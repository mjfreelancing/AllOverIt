using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Configuration
{
    public sealed record GraphqlClientConfiguration
    {
        public string EndPoint { get; init; }
        public IJsonSerializer Serializer { get; init; }
        public IAppSyncAuthorization DefaultAuthorization { get; init; }
    }
}