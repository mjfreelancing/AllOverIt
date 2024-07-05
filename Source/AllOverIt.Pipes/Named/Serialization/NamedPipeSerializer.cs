using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Writers;
using System.Text;

namespace AllOverIt.Pipes.Named.Serialization
{
    /// <summary>Provides binary serialization support for a named pipe. Reading is performed using <see cref="EnrichedBinaryValueReader"/>
    /// and writing is performed using <see cref="EnrichedBinaryValueWriter"/>. If a custom reader or writer is not provided for any property
    /// type (or even <typeparamref name="TMessage"/>) then a <see cref="DynamicBinaryValueReader"/> or <see cref="DynamicBinaryValueWriter"/> will be
    /// used as required.</summary>
    /// <typeparam name="TMessage">The message type to be serialized.</typeparam>
    public class NamedPipeSerializer<TMessage> : INamedPipeSerializer<TMessage>
        where TMessage : class, new()
    {
        /// <summary>Contains custom value readers for <typeparamref name="TMessage"/> or any of its' property types. If the serializer
        /// encounters a type for which there is no <see cref="IEnrichedBinaryValueReader"/> then a <see cref="DynamicBinaryValueReader"/>
        /// will be used.</summary>
        public ICollection<IEnrichedBinaryValueReader> Readers { get; } = [];

        /// <summary>Contains custom value writer for <typeparamref name="TMessage"/> or any of its' property types. If the serializer
        /// encounters a type for which there is no <see cref="IEnrichedBinaryValueWriter"/> then a <see cref="DynamicBinaryValueWriter"/>
        /// will be used.</summary>
        public ICollection<IEnrichedBinaryValueWriter> Writers { get; } = [];

        /// <inheritdoc/>
        public byte[] Serialize(TMessage message)
        {
            _ = message.WhenNotNull();

            using (var stream = new MemoryStream())
            {
                using (var serializerWriter = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    foreach (var writer in Writers)
                    {
                        serializerWriter.Writers.Add(writer);
                    }

                    serializerWriter.WriteObject(message, typeof(TMessage));
                }

                return stream.ToArray();
            }
        }

        /// <inheritdoc/>
        public TMessage? Deserialize(byte[]? bytes)
        {
            // If a connection is broken we get back an empty byte array - we don't want to throw
            if (bytes.IsNullOrEmpty())
            {
                return null;
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var serializerReader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    foreach (var reader in Readers)
                    {
                        serializerReader.Readers.Add(reader);
                    }

                    return (TMessage) serializerReader.ReadObject();
                }
            }
        }
    }
}