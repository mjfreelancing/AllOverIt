namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>Contains all connection states applicable to a subscription registration.</summary>
    public enum SubscriptionConnectionState
    {
        /// <summary>A new connection to AppSync has begun.</summary>
        Connecting,

        /// <summary>A connection has been established with AppSync and is now ready for subscription registrations.</summary>
        Connected,

        /// <summary>Indicates the subscription registration is still healthy.</summary>
        KeepAlive,

        /// <summary>The websocket connection is disconnecting either explicitly or due to an error.</summary>
        Disconnecting,

        /// <summary>The websocket connection has disconnected from AppSync.</summary>
        Disconnected,

        /// <summary>A <see cref="KeepAlive"/> message has not been received so the connection is being re-established.</summary>
        ConnectionReset
    }
}