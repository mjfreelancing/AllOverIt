using AllOverIt.Collections;
using System;
using System.Collections.Generic;

namespace KeysetPaginationConsole.KeysetPagination
{
    internal sealed class ContinuationToken
    {
        internal static readonly ContinuationToken None = new();

        internal sealed class ValueType
        {
            public TypeCode TypeCode { get; init; }
            public object Value { get; init; }
        }

        // Indicates the direction to move based on the ValueTypes to get the required page represented by this token (previous or next)
        public PaginationDirection Direction { get; init; } = PaginationDirection.Forward;

        // The reference values that identifies the row to traverse from
        public IReadOnlyCollection<ValueType> ValueTypes { get; init; } = List.EmptyReadOnly<ValueType>();
    }
}
