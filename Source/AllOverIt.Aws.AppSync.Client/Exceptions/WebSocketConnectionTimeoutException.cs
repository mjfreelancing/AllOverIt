using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class WebSocketConnectionTimeoutException : TimeoutExceptionBase
    {
        public WebSocketConnectionTimeoutException(TimeSpan timeout)
            : base(timeout)
        {
        }
    }
}