namespace AllOverIt.Pagination
{
    /// <summary>Contains information that can be serialized to enable paginated queries.</summary>
    public interface IContinuationToken
    {
        /// <summary>Indicates the direction to move based on the reference <see cref="Values"/> in order to get
        /// the required previous or next page.</summary>
        PaginationDirection Direction { get; }

        /// <summary>The reference (column) values that identifies the record (row) to traverse from.</summary>
        object?[]? Values { get; }
    }
}
