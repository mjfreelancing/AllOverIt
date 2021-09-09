namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal sealed class ConnectionAckResponseData
    {
        // Time in milliseconds waiting for ka message before the client should terminate the WebSocket connection
        public int ConnectionTimeoutMs { get; set; }
    }
}