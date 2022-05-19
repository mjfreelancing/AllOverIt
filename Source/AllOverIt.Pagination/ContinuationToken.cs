using AllOverIt.Collections;
using System;
using System.Collections.Generic;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationToken
    {
        internal static readonly ContinuationToken None = new();

        internal sealed class ValueType
        {
            public TypeCode Type { get; init; }
            public object Value { get; init; }
        }

        // Indicates the direction to move based on the reference 'Values' in order to get the required previous or next page.
        public PaginationDirection Direction { get; init; }

        // The reference values that identifies the row to traverse from.
        public IReadOnlyCollection<ValueType> Values { get; init; }
    }
}
