namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class WebSocketSubscriptionResponse<TPayload> : WebSocketResponse<TPayload>
    {
        public string Id { get; set; }
    }
}