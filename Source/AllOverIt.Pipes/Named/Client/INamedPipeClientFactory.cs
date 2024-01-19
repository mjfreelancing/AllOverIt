namespace AllOverIt.Pipes.Named.Client
{
    /// <summary>Represents a factory that creates instances of a named pipe client.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipeClientFactory<TMessage> where TMessage : class, new()
    {
        /// <summary>Creates a named pipe client using the provided <paramref name="pipeName"/> that will connect to a local
        /// named pipe server.</summary>
        /// <param name="pipeName">The name of the pipe.</param>
        /// <returns>A new named pipe client instance.</returns>
        INamedPipeClient<TMessage> CreateNamedPipeClient(string pipeName);

        /// <summary>Creates a named pipe client using the provided <paramref name="pipeName"/> that will connect to a named
        /// pipe on the server specified by <paramref name="serverName"/>.</summary>
        /// <param name="pipeName">The name of the pipe.</param>
        /// <param name="serverName">The server name the named pipe client will connect to.</param>
        /// <returns>A new named pipe client instance.</returns>
        INamedPipeClient<TMessage> CreateNamedPipeClient(string pipeName, string serverName);
    }
}