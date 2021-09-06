using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class GraphqlConnectionTimeoutException : GraphqlTimeoutExceptionBase
    {
        public GraphqlConnectionTimeoutException(TimeSpan timeoutPeriod)
            : base($"Failed to make a websocket connection within {timeoutPeriod.TotalMilliseconds}ms.", timeoutPeriod)
        {
        }
    }
}