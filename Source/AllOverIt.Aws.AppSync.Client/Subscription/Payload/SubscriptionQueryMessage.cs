namespace AllOverIt.Aws.AppSync.Client.Subscription.Payload
{
    internal class SubscriptionQueryMessage
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public SubscriptionQueryPayload Payload { get; set; }
    }
}