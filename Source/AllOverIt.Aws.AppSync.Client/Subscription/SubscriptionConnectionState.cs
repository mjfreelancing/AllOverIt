namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public enum SubscriptionConnectionState
    {
        Connecting,         // Beginning a new connection to AppSync.
        Connected,          // Connection is established with AppSync and now ready for subscriptions.
        KeepAlive,          // A healthy status check.
        Disconnecting,      // Disconnecting either explicitly or due to an error.
        Disconnected,       // Disconnected from AppSync.
        ConnectionReset     // A health status message has not been received so the connection is being re-established.
    }
}