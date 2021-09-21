namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal sealed class ConnectionAckResponse
    {
        // Time in milliseconds waiting for ka (keep alive) message before the client should terminate the WebSocket connection
        public int ConnectionTimeoutMs { get; init; }
    }
}