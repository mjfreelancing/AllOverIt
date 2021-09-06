using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class GraphqlSubscribeTimeoutException : GraphqlTimeoutExceptionBase
    {
        public GraphqlSubscribeTimeoutException(string id, TimeSpan timeoutPeriod)
            : base($"Failed to subscribe '{id}' within {timeoutPeriod.TotalMilliseconds}ms.", timeoutPeriod)
        {
        }
    }
}