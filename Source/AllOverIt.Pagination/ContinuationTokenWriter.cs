using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationTokenWriter : EnrichedBinaryValueWriter<ContinuationToken>
    {
        private static readonly Type ValuesType = typeof(IReadOnlyCollection<object>);

        public override void WriteValue(EnrichedBinaryWriter writer, object value)
        {
            var continuationToken = (ContinuationToken) value;

            writer.WriteByte((byte) continuationToken.Direction);

            // Not using writer.WriteEnumerable() as this assumes at least one value and the token Values can be null (when encoding the first/last page).
            // We could use continuationToken.Values ?? Array.Empty<object>() but then the reader would construct a ContinuationToken with an array of zero
            // values rather than null - resulting in decoding not matching the original value.
            writer.WriteObject(ValuesType, continuationToken.Values);
        }
    }

}
