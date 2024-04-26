namespace AllOverIt.Pagination
{
    internal sealed class ContinuationToken : IContinuationToken
    {
        public static readonly ContinuationToken None = new();

        public PaginationDirection Direction { get; init; }
        public IReadOnlyCollection<object> Values { get; init; }
    }
}
