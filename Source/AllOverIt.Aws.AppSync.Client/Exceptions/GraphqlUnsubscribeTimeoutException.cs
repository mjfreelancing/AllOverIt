using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class GraphqlUnsubscribeTimeoutException : GraphqlSubscriptionTimeoutExceptionBase
    {
        public GraphqlUnsubscribeTimeoutException(string id, TimeSpan timeout)
            : base($"Failed to unsubscribe '{id}' within {timeout.TotalMilliseconds}ms.", id, timeout)
        {
        }
    }
}