using AllOverIt.Filtering.Filters;

namespace EFEnumerationDemo
{
    public class BlogFilter : IFilter
    {
        public EqualTo<int> EqualToInt { get; init; } = new();
        public EqualTo<string> EqualToString { get; init; } = new();
        public GreaterThan<int> GreaterThan { get; init; } = new();
        public GreaterThanOrEqual<int> GreaterThanOrEqual { get; init; } = new();
        public LessThan<int> LessThan { get; init; } = new();
        public LessThanOrEqual<int> LessThanOrEqual { get; init; } = new();
        public Contains Contains { get; init; } = new();
        public StartsWith StartsWith { get; init; } = new();
        public EndsWith EndsWith { get; init; } = new();
        public In<int> In { get; init; } = new();
    }
}