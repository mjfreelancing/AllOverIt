using AllOverIt.Assertion;
using AllOverIt.Serialization.Binary.Readers;

namespace AllOverIt.Serialization.Binary.Readers.Extensions
{
    /// <summary>Provides extension methods for <see cref="IEnrichedBinaryValueReader"/>.</summary>
    public static class EnrichedBinaryValueReaderExtensions
    {
        /// <summary>Reads the content of the underlying stream as a <typeparamref name="TValue"/>.</summary>
        /// <typeparam name="TValue">The type to cast the read value.</typeparam>
        /// <param name="valueReader">The binary value reader that will read the value from the underlying stream.</param>
        /// <param name="reader">The binary reader that will read the value from the underlying stream.</param>
        /// <returns>The value read from the stream cast to a <typeparamref name="TValue"/>.</returns>
        public static TValue ReadValue<TValue>(this IEnrichedBinaryValueReader valueReader, IEnrichedBinaryReader reader)
        {
            _ = valueReader.WhenNotNull(nameof(valueReader));
            _ = reader.WhenNotNull(nameof(reader));

            return (TValue) valueReader.ReadValue(reader);
        }
    }
}