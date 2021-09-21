namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    /// <summary>Contains subscription registration details to be sent to AppSync.</summary>
    internal sealed class SubscriptionQueryPayload
    {
        /// <summary>A string representation of the subscription's query and variable definition.</summary>
        public string Data { get; init; }

        /// <summary>Contains additional information to be sent along with the registration request, such as authorization headers.</summary>
        public object Extensions { get; init; }
    }
}