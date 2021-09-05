using AllOverIt.Helpers;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class GraphqlSubscriptionResponseError
    {
        public string Id { get; }
        public WebSocketResponse<GraphqlError> Error { get; }

        public GraphqlSubscriptionResponseError(string id, WebSocketResponse<GraphqlError> error)
        {
            Id = id;        // will be null if it is an immediate connection error at the time of subscription
            Error = error.WhenNotNull(nameof(error));
        }
    }
}