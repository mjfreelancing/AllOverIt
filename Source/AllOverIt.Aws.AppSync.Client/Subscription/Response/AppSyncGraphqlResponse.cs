using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public class AppSyncGraphqlResponse : GraphqlWebSocketResponse
    {

        [IgnoreDataMember]
        public string Message { get; set; }
    }
}