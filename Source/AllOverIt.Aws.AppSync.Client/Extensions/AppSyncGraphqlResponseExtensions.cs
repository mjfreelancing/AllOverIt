using AllOverIt.Aws.AppSync.Client.Subscription;
using AllOverIt.Aws.AppSync.Client.Subscription.Response;
using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    internal static class AppSyncGraphqlResponseExtensions
    {
        public static WebSocketGraphqlResponse<GraphqlError> GetGraphqlErrorFromResponseMessage(this AppSyncGraphqlResponse response, IJsonSerializer serializer)
        {
            return serializer.DeserializeObject<WebSocketGraphqlResponse<GraphqlError>>(response.Message);
        }

        public static ConnectionAckResponse GetConnectionResponseData(this AppSyncGraphqlResponse response, IJsonSerializer serializer)
        {
            return serializer.DeserializeObject<WebSocketGraphqlResponse<ConnectionAckResponse>>(response.Message).Payload;
        }
    }
}