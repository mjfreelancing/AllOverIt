namespace AllOverIt.Pagination
{
    internal sealed class ContinuationToken : IContinuationToken
    {
        public static readonly ContinuationToken None = new();

        public PaginationDirection Direction { get; init; }
        public object?[]? Values { get; init; }
    }
}
