using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>This exception is raised when an attempt to make a new websocket connection times out within a specified period.</summary>
    [Serializable]
    public sealed class WebSocketConnectionTimeoutException : TimeoutExceptionBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="timeout">The timeout period.</param>
        public WebSocketConnectionTimeoutException(TimeSpan timeout)
            : base(timeout)
        {
        }
    }
}