namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>Contains a query message for a subscription handshake.</summary>
    internal class SubscriptionQueryMessage
    {
        /// <summary>When applicable to a subscription, this is the registered identifier.</summary>
        public string Id { get; init; }

        /// <summary>The message type. See <see cref="ProtocolMessage.Request"/> for possible values.</summary>
        public string Type { get; init; }

        /// <summary>The message payload to be sent to AppSync.</summary>
        public SubscriptionQueryPayload Payload { get; init; }
    }
}