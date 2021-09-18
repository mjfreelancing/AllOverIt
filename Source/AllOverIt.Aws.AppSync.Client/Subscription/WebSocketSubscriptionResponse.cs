using AllOverIt.Aws.AppSync.Client.Response;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal sealed class WebSocketSubscriptionResponse<TPayload> : WebSocketGraphqlResponse<TPayload>
    {
        public string Id { get; init; }
    }
}