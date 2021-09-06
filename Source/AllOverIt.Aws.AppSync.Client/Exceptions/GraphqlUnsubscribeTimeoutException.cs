using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class GraphqlUnsubscribeTimeoutException : GraphqlTimeoutExceptionBase
    {
        public GraphqlUnsubscribeTimeoutException(string id, TimeSpan timeoutPeriod)
            : base($"Failed to unsubscribe '{id}' within {timeoutPeriod.TotalMilliseconds}ms.", timeoutPeriod)
        {
        }
    }
}