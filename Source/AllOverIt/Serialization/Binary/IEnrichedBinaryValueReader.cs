using System;

namespace AllOverIt.Serialization.Binary
{
    /// <summary>Represents a binary value reader for a given object type. When registered as an <see cref="IEnrichedBinaryReader"/> reader,
    /// these readers make it possible to deserialize complex object graphs.</summary>
    public interface IEnrichedBinaryValueReader
    {
        /// <summary>The object type being read from the underlying stream.</summary>
        Type Type { get; }

        /// <summary>Reads the content of the underlying stream as a <see cref="IEnrichedBinaryValueReader.Type"/>.</summary>
        /// <param name="reader">The binary reader that will read the value from the underlying stream.</param>
        /// <returns>The object read from the stream.</returns>
        object ReadValue(IEnrichedBinaryReader reader);

        /// <summary>Reads the content of the underlying stream as a <see cref="IEnrichedBinaryValueReader.Type"/>.</summary>
        /// <typeparam name="TValue">The type to cast the read value. It should be the same as <see cref="IEnrichedBinaryValueReader.Type"/>.</typeparam>
        /// <param name="reader">The binary reader that will read the value from the underlying stream.</param>
        /// <returns>The object read from the stream cast to a <typeparamref name="TValue"/>.</returns>
        TValue ReadValue<TValue>(IEnrichedBinaryReader reader);
    }
}
