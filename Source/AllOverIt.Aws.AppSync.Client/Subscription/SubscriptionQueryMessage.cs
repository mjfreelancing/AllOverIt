namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal class SubscriptionQueryMessage
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public SubscriptionQueryPayload Payload { get; set; }
    }
}