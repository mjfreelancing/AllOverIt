namespace AllOverIt.Pipes.Named.Server
{
    /// <summary>Represents a factory that creates instances of a named pipe server.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipeServerFactory<TMessage> where TMessage : class, new()
    {
        /// <summary>Creates a named pipe server using the provided <paramref name="pipeName"/> that will connect to a
        /// named pipe server.</summary>
        /// <param name="pipeName">The name of the pipe.</param>
        /// <returns>A new named pipe server instance.</returns>
        INamedPipeServer<TMessage> CreateNamedPipeServer(string pipeName);
    }
}