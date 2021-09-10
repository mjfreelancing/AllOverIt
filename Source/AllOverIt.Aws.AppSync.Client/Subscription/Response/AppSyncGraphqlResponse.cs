using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public sealed class AppSyncGraphqlResponse : GraphqlWebSocketResponse
    {

        [IgnoreDataMember]
        public string Message { get; set; }
    }
}