using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class SubscribeTimeoutException : SubscriptionTimeoutExceptionBase
    {
        public SubscribeTimeoutException(string id, TimeSpan timeout)
            : base(id, timeout)
        {
        }
    }
}