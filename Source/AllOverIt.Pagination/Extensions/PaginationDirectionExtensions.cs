namespace AllOverIt.Pagination.Extensions
{
    public static class PaginationDirectionExtensions
    {
        public static PaginationDirection Reverse(this PaginationDirection direction)
        {
            return direction == PaginationDirection.Forward
                ? PaginationDirection.Backward
                : PaginationDirection.Forward;
        }
    }
}
