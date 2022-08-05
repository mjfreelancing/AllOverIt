using AllOverIt.Filtering.Filters;

namespace PaginationConsole
{
    internal sealed class BlogFilter
    {
        public sealed class DescriptionFilter
        {
            public Contains Contains { get; set; } = new();
            public StartsWith StartsWith { get; set; } = new();
        }

        public DescriptionFilter Description { get; init; } = new();
    }
}