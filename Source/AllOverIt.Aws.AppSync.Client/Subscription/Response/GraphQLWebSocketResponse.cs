namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public abstract class GraphqlWebSocketResponse
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}