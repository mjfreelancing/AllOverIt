using AllOverIt.Assertion;

namespace AllOverIt.Serialization.Binary.Writers.Extensions
{
    /// <summary>Provides extension methods for <see cref="IEnrichedBinaryValueWriter"/>.</summary>
    public static class EnrichedBinaryValueWriterExtensions
    {
        /// <summary>Writes the provided value to the underlying stream.</summary>
        /// <typeparam name="TValue">The value type to be written.</typeparam>
        /// <param name="valueWriter">The binary value writer that will write the value to the underlying stream.</param>
        /// <param name="writer">The binary writer that will write the value to the underlying stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteValue<TValue>(this IEnrichedBinaryValueWriter valueWriter, IEnrichedBinaryWriter writer, TValue value)
        {
            _ = valueWriter.WhenNotNull(nameof(valueWriter));
            _ = writer.WhenNotNull(nameof(writer));

            valueWriter.WriteValue(writer, value);
        }
    }
}