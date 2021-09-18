namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public abstract class GraphqlWebSocketResponse
    {
        public string Id { get; init; }
        public string Type { get; init; }
    }
}