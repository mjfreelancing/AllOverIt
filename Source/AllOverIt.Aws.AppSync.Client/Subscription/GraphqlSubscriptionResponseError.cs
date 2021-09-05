using AllOverIt.Helpers;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class GraphqlSubscriptionResponseError
    {
        public string Id { get; }
        public WebSocketResponse<GraphqlError> Error { get; }

        public GraphqlSubscriptionResponseError(string id, WebSocketResponse<GraphqlError> error)
        {
            Id = id.WhenNotNullOrEmpty(nameof(id));
            Error = error.WhenNotNull(nameof(error));
        }
    }
}