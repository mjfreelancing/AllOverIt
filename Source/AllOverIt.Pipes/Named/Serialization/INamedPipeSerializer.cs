namespace AllOverIt.Pipes.Named.Serialization
{
    /// <summary>Represents a binary message serializer for a named pipe.</summary>
    /// <typeparam name="TMessage">The message type to be serialized.</typeparam>
    public interface INamedPipeSerializer<TMessage>
    {
        /// <summary>Serializes a <typeparamref name="TMessage"/> to a byte array.</summary>
        /// <param name="message">The message to be serialized.</param>
        /// <returns>The message serialized to a byte array.</returns>
        public byte[] Serialize(TMessage message);

        /// <summary>Deserializes a byte array to a <typeparamref name="TMessage"/> instance.</summary>
        /// <param name="bytes">The serialized byte array.</param>
        /// <returns>An initialized instance of the required type.</returns>
        public TMessage Deserialize(byte[] bytes);
    }
}