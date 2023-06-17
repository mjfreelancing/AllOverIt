﻿using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using System;
using System.IO;

namespace AllOverIt.Pipes.Named.Server
{
    public interface IPipeServerConnection<TMessage> : IConnectablePipeConnection<TMessage>
    {
        /// <summary>Event raised when a message is received from the other end of the pipe. Messages are raised
        /// on a background task.</summary>
        public event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnMessageReceived;

        /// <summary>Event raised when the pipe connection is disconnected. This event is raised on a background task.</summary>
        public event EventHandler<ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnDisconnected;

        /// <summary>Event raised when an exception is thrown during any read/write operation. This event is raised on a
        /// background task.</summary>
        public event EventHandler<ConnectionExceptionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnException;

        /// <summary>Gets the username of the connected client. The client's username will not be available until it has 
        /// written at least once to the pipe (and has set its impersonation level appropriately).</summary>
        /// <returns>The username of the connected client.</returns>
        /// <exception cref="IOException">If the client has not yet written to the pipe.</exception>
        string GetImpersonationUserName();
    }
}