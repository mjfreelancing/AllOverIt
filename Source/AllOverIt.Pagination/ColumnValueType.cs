using System;

namespace AllOverIt.Pagination
{
    [Serializable]
    internal sealed class ColumnValueType
    {
        public TypeCode Type { get; init; }
        public object Value { get; init; }
    }
}
