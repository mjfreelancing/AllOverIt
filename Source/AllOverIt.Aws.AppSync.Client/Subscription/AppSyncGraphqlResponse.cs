using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal sealed class AppSyncGraphqlResponse
    {
        public string Id { get; init; }
        public string Type { get; init; }

        [IgnoreDataMember]
        public string Message { get; internal set; }
    }
}