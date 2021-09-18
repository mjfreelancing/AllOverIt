namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public sealed class WebSocketSubscriptionResponse<TPayload> : WebSocketGraphqlResponse<TPayload>
    {
        public string Id { get; init; }
    }
}