namespace AllOverIt.Pagination.Extensions
{
    internal static class PaginationDirectionExtensions
    {
        public static PaginationDirection Reverse(this PaginationDirection direction)
        {
            return direction == PaginationDirection.Forward
                ? PaginationDirection.Backward
                : PaginationDirection.Forward;
        }
    }
}
