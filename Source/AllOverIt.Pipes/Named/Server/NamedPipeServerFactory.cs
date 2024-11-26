using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Serialization;

namespace AllOverIt.Pipes.Named.Server
{
    /// <summary>A factory that creates instances of a named pipe server.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public sealed class NamedPipeServerFactory<TMessage> : INamedPipeServerFactory<TMessage> where TMessage : class, new()
    {
        private readonly INamedPipeSerializer<TMessage> _serializer;

        /// <summary>Constructor.</summary>
        /// <param name="serializer">The message serializer.</param>
        public NamedPipeServerFactory(INamedPipeSerializer<TMessage> serializer)
        {
            _serializer = serializer.WhenNotNull();
        }

        /// <inheritdoc />
        public INamedPipeServer<TMessage> CreateNamedPipeServer(string pipeName)
        {
            return new NamedPipeServer<TMessage>(pipeName, _serializer);
        }
    }
}