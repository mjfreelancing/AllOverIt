namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>Contains a graphql subscription message from AppSync.</summary>
    internal sealed class AppSyncSubscriptionMessage
    {
        /// <summary>The subscription identifier this message is related to.</summary>
        public required string Id { get; init; }

        /// <summary>The message type. See <see cref="ProtocolMessage.Response"/> for possible values.</summary>
        public required string Type { get; init; }

        /// <summary>The content of the message received from AppSync.</summary>
        public required string Message { get; internal set; }
    }
}