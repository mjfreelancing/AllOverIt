using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public class AppSyncGraphqlResponse : GraphQLWebSocketResponse
    {

        [IgnoreDataMember]
        public string Message { get; set; }
    }
}