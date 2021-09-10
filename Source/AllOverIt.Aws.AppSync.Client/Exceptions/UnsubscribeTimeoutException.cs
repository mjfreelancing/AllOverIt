using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class UnsubscribeTimeoutException : SubscriptionTimeoutExceptionBase
    {
        public UnsubscribeTimeoutException(string id, TimeSpan timeout)
            : base(id, timeout)
        {
        }
    }
}