using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class GraphqlSubscribeTimeoutException : GraphqlSubscriptionTimeoutExceptionBase
    {
        public GraphqlSubscribeTimeoutException(string id, TimeSpan timeout)
            : base($"Failed to subscribe '{id}' within {timeout.TotalMilliseconds}ms.", id, timeout)
        {
        }
    }
}