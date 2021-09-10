﻿namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    internal sealed class ConnectionAckResponse
    {
        // Time in milliseconds waiting for ka message before the client should terminate the WebSocket connection
        public int ConnectionTimeoutMs { get; set; }
    }
}