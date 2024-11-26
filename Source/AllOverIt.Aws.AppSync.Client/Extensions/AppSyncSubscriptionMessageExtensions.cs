using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Aws.AppSync.Client.Subscription;
using AllOverIt.Serialization.Json.Abstractions;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    internal static class AppSyncSubscriptionMessageExtensions
    {
        internal static WebSocketGraphqlResponse<GraphqlError> GetGraphqlErrorFromResponseMessage(this AppSyncSubscriptionMessage response, IJsonSerializer serializer)
        {
            return serializer.DeserializeObject<WebSocketGraphqlResponse<GraphqlError>>(response.Message)!;
        }

        internal static ConnectionAckResponse GetConnectionResponseData(this AppSyncSubscriptionMessage response, IJsonSerializer serializer)
        {
            return serializer.DeserializeObject<WebSocketGraphqlResponse<ConnectionAckResponse>>(response.Message)!.Payload;
        }
    }
}