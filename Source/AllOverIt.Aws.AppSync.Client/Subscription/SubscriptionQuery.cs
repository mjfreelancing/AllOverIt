namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class SubscriptionQuery
    {
        public string Query { get; set; }
        public object Variables { get; set; }
    }
}