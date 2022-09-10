using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Pagination.TokenEncoding
{
    internal sealed class ContinuationTokenReader : EnrichedBinaryValueReader<ContinuationToken>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            _ = reader.WhenNotNull(nameof(reader));

            var direction = (PaginationDirection) reader.ReadByte();

            // Not using reader.ReadEnumerable() as this would assume at least one value and the token Values can be null.
            // ReadObject(), when provided a type, caters for null values.
            var values = reader.ReadObject<IEnumerable<object>>()?.AsReadOnlyCollection();

            return new ContinuationToken
            {
                Direction = direction,
                Values = values
            };
        }
    }

}
