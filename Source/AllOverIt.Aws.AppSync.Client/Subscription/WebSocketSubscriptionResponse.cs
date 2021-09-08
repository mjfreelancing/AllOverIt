namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class WebSocketSubscriptionResponse<TPayload> : WebSocketGraphqlResponse<TPayload>
    {
        public string Id { get; set; }
    }
}