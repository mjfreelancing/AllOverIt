using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Server;

namespace AllOverIt.Pipes.Named.Events
{
    /// <summary>Defines pipe connection related event arguments.</summary>
    /// <typeparam name="TMessage">The message type serialized on the pipe connection.</typeparam>
    /// <typeparam name="TPipeConnection">The connection type. This will be <see cref="INamedPipeServerConnection{TMessage}"/> or
    /// <see cref="INamedPipeClientConnection{TMessage}"/>.</typeparam>
    public class NamedPipeConnectionEventArgs<TMessage, TPipeConnection> : EventArgs
        where TPipeConnection : class, INamedPipeConnection<TMessage>
        where TMessage : class, new()
    {
        /// <summary>The connection associated with the event.</summary>
        public TPipeConnection Connection { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        public NamedPipeConnectionEventArgs(TPipeConnection connection)
        {
            Connection = connection.WhenNotNull();
        }
    }
}