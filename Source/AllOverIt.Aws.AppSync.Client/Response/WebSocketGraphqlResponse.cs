using AllOverIt.Aws.AppSync.Client.Subscription;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    // https://docs.aws.amazon.com/appsync/latest/devguide/real-time-websocket-client.html#error-message

    /// <summary>A wrapper class that contains the payload for a websocket response.</summary>
    /// <typeparam name="TPayload">The payload type wrapped within the response.</typeparam>
    public class WebSocketGraphqlResponse<TPayload>
    {
        /// <summary>The response type. See <see cref="ProtocolMessage.Response"/> for possible values.</summary>
        public required string Type { get; init; }

        /// <summary>The payload of the response.</summary>
        public required TPayload Payload { get; init; }
    }
}