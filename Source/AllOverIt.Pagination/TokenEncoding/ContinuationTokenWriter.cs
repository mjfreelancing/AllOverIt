using AllOverIt.Assertion;
using AllOverIt.Serialization.Binary.Writers;

namespace AllOverIt.Pagination.TokenEncoding
{
    internal sealed class ContinuationTokenWriter : EnrichedBinaryValueWriter<ContinuationToken>
    {
        private static readonly Type ValuesType = typeof(IReadOnlyCollection<object>);

        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            _ = writer.WhenNotNull(nameof(writer));

            var continuationToken = (ContinuationToken) value;

            // Could also use the WriteByte() extension
            writer.Write((byte) continuationToken.Direction);

            // Not using writer.WriteEnumerable() as this assumes at least one value and the token Values can be null (when encoding the first/last page).
            // We could use continuationToken.Values ?? Array.Empty<object>() but then the reader would construct a ContinuationToken with an array of zero
            // values rather than null - resulting in decoding not matching the original value.
            writer.WriteObject(continuationToken.Values, ValuesType);
        }
    }
}
