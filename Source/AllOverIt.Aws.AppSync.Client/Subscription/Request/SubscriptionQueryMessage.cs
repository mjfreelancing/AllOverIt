namespace AllOverIt.Aws.AppSync.Client.Subscription.Request
{
    internal class SubscriptionQueryMessage
    {
        public string Id { get; init; }
        public string Type { get; init; }
        public SubscriptionQueryPayload Payload { get; init; }
    }
}