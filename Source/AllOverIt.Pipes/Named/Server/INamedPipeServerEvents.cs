using AllOverIt.Pipes.Named.Events;

namespace AllOverIt.Pipes.Named.Server
{
    /// <summary>Declares event handlers specific to a named pipe server.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipeServerEvents<TMessage> where TMessage : class, new()
    {
        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>>? OnClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>>? OnClientDisconnected;

        /// <summary>
        /// Invoked whenever a message is received.
        /// </summary>
        event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>>>? OnMessageReceived;
    }
}