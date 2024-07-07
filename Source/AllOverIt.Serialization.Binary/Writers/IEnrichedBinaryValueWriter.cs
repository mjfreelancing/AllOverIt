namespace AllOverIt.Serialization.Binary.Writers
{
    /// <summary>Represents a binary value writer for a given object type. When registered as an <see cref="IEnrichedBinaryWriter"/> writer,
    /// these writers make it possible to serialize complex object graphs.</summary>
    public interface IEnrichedBinaryValueWriter
    {
        /// <summary>The object type being written to the underlying stream.</summary>
        Type Type { get; }

        /// <summary>Writes the provided value to the underlying stream. It is expected the <paramref name="value"/> is of the required
        /// <see cref="Type"/>.</summary>
        /// <param name="writer">The binary writer that will write the value to the underlying stream.</param>
        /// <param name="value">The object value to be written. The value cannot be <see langword="null"/>.</param>
        void WriteValue(IEnrichedBinaryWriter writer, object value);
    }
}
