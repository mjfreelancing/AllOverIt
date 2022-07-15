using AllOverIt.Filtering.Filters;

namespace EFEnumerationDemo
{
    public class BlogFilter : IFilter
    {
        public GreaterThan<int> GreaterThan { get; init; } = new();
        public LessThan<int> LessThan { get; init; } = new();
        public Contains Contains { get; init; } = new();
        public StartsWith StartsWith { get; init; } = new();
    }
}