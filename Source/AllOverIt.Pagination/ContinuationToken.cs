using System;
using System.Collections.Generic;

namespace AllOverIt.Pagination
{
    [Serializable]
    internal sealed class ContinuationToken
    {
        [field: NonSerialized]
        public static readonly ContinuationToken None = new();

        // Indicates the direction to move based on the reference 'Values' in order to get the required previous or next page.
        public PaginationDirection Direction { get; init; }

        // The reference values that identifies the row to traverse from.
        public IReadOnlyCollection<object> Values { get; init; }
    }
}
