namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>This exception is raised when a subscription request does not complete within a period defined on the connection options.</summary>
    public sealed class SubscribeTimeoutException : SubscriptionTimeoutExceptionBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="id">The unique subscription Id.</param>
        /// <param name="timeout">The timeout period that the subscription was expected to be completed within.</param>
        internal SubscribeTimeoutException(string id, TimeSpan timeout)
            : base(id, timeout)
        {
        }
    }
}