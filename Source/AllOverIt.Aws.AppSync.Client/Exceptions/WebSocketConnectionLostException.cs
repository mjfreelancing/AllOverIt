using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>This exception is raised when there was an expectation that a WebSocket is available but found to be
    /// unavailable (due to an error that caused the connection to be shutdown).</summary>
    public sealed class WebSocketConnectionLostException : Exception
    {
    }
}