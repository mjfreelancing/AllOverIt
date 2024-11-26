using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Serialization;

namespace AllOverIt.Pipes.Named.Client
{
    /// <summary>Represents a factory that creates instances of a named pipe client.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public sealed class NamedPipeClientFactory<TMessage> : INamedPipeClientFactory<TMessage> where TMessage : class, new()
    {
        private readonly INamedPipeSerializer<TMessage> _serializer;

        /// <summary>Constructor.</summary>
        /// <param name="serializer">The serializer to be used by named pipe client instances.</param>
        public NamedPipeClientFactory(INamedPipeSerializer<TMessage> serializer)
        {
            _serializer = serializer.WhenNotNull();
        }

        /// <inheritdoc/>
        public INamedPipeClient<TMessage> CreateNamedPipeClient(string pipeName)
        {
            _ = pipeName.WhenNotNullOrEmpty();

            return new NamedPipeClient<TMessage>(pipeName, _serializer);
        }

        /// <inheritdoc/>
        public INamedPipeClient<TMessage> CreateNamedPipeClient(string pipeName, string serverName)
        {
            _ = pipeName.WhenNotNullOrEmpty();
            _ = serverName.WhenNotNullOrEmpty();

            return new NamedPipeClient<TMessage>(pipeName, serverName, _serializer);
        }
    }
}