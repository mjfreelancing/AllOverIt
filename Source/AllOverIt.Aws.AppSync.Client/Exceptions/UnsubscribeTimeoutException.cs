namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>This exception is raised when a request to unsubscribe a subscription does not complete within a period defined on the connection options.</summary>
    [Serializable]
    public sealed class UnsubscribeTimeoutException : SubscriptionTimeoutExceptionBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="id">The unique subscription Id.</param>
        /// <param name="timeout">The timeout period that the unsubscribe was expected to be completed within.</param>
        public UnsubscribeTimeoutException(string id, TimeSpan timeout)
            : base(id, timeout)
        {
        }
    }
}