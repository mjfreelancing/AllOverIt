using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using System;

namespace AllOverIt.Pipes.Named.Client
{
    /// <summary>Represents a named pipe client connection to a named pipe server.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipeClientConnection<TMessage> : IConnectableNamedPipeConnection<TMessage> where TMessage : class, new()
    {
        /// <summary>Event raised when a message is received from the other end of the pipe. Messages are raised
        /// on a background task.</summary>
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <summary>Event raised when the pipe connection is disconnected. This event is raised on a background task.</summary>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnDisconnected;

        /// <summary>Event raised when an exception is thrown during any read/write operation. This event is raised on a
        /// background task.</summary>
        public event EventHandler<NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnException;

        /// <summary>Gets the connection's server name.</summary>
        public string ServerName { get; }
    }
}