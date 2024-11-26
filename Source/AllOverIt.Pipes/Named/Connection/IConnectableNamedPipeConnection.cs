namespace AllOverIt.Pipes.Named.Connection
{
    /// <summary>A <see cref="INamedPipeConnection{TMessage}"/> that can be connected and disconnected.</summary>
    /// <typeparam name="TMessage">The message type serialized by the connection.</typeparam>
    public interface IConnectableNamedPipeConnection<TMessage> : INamedPipeConnection<TMessage>
    {
        /// <summary>Connects to an underlying pipe stream.</summary>
        void Connect();

        /// <summary>Disconnects from an underlying pipe stream.</summary>
        Task DisconnectAsync();
    }
}