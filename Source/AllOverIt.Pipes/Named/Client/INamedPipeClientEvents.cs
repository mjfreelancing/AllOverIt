using AllOverIt.Pipes.Named.Events;
using System;

namespace AllOverIt.Pipes.Named.Client
{
    /// <summary>Declares event handlers specific to a named pipe client.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipeClientEvents<TMessage> where TMessage : class, new()
    {
        /// <summary>Event raised when the named pipe client connects to the named pipe server.</summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnConnected;

        /// <summary>Event raised when the named pipe client disconnects from the named pipe server.</summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnDisconnected;

        /// <summary>Event raised when the named pipe client receives a message from the named pipe server.</summary>
        event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnMessageReceived;
    }
}