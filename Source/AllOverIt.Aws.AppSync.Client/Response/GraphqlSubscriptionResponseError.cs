using AllOverIt.Assertion;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    /// <summary>Contains error information for a subscription query.</summary>
    public sealed class GraphqlSubscriptionResponseError
    {
        /// <summary>The unique identifier of the failed subscription request. This will be <see langword="null"/> if
        /// there is an immediate connection error at the time of subscription.</summary>
        public string? Id { get; }

        /// <summary>Contains detailed error information reported by AppSync.</summary>
        public WebSocketGraphqlResponse<GraphqlError> Error { get; }

        /// <summary>Constructor.</summary>
        /// <param name="id">The unique identifier of the failed subscription request.</param>
        /// <param name="error">Detailed error information reported by AppSync.</param>
        public GraphqlSubscriptionResponseError(string? id, WebSocketGraphqlResponse<GraphqlError> error)
        {
            Id = id;        // will be null if it is an immediate connection error at the time of subscription
            Error = error.WhenNotNull();
        }
    }
}