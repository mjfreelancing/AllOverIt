namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal sealed class SubscriptionQueryPayload
    {
        public string Data { get; init; }        // string representation of query and variables
        public object Extensions { get; init; }
    }
}