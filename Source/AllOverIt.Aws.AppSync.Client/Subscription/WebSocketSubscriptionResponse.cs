using AllOverIt.Aws.AppSync.Client.Response;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>A wrapper class that contains the payload for a websocket subscription response.</summary>
    /// <typeparam name="TPayload">The payload type wrapped within the subscription response.</typeparam>
    internal sealed class WebSocketSubscriptionResponse<TPayload> : WebSocketGraphqlResponse<TPayload>
    {
        /// <summary>The subscription identifier this response is associated with.</summary>
        public required string Id { get; init; }
    }
}