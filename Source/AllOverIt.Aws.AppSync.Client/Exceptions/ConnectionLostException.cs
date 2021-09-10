using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    // Will be raised if there was an expectation that the WebSocket is available but found to be unavailable
    // (due to an error that caused it to be shutdown)
    public sealed class ConnectionLostException : Exception
    {
    }
}