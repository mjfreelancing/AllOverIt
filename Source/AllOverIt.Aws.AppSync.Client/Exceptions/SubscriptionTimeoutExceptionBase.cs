namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>Base class for all subscription related timeout exceptions.</summary>
    public abstract class SubscriptionTimeoutExceptionBase : TimeoutExceptionBase
    {
        /// <summary>The unique Id for the subscription that timed out.</summary>
        public string Id { get; }

        /// <summary>Constructor.</summary>
        /// <param name="id">The unique Id for the subscription that timed out.</param>
        /// <param name="timeout">The timeout period that expired.</param>
        protected SubscriptionTimeoutExceptionBase(string id, TimeSpan timeout)
            : base(timeout)
        {
            Id = id;
        }
    }
}