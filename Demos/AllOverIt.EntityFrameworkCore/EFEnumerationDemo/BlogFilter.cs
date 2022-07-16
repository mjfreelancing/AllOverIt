using AllOverIt.Filtering.Filters;

namespace EFEnumerationDemo
{
    public class BlogFilter : IFilter
    {
        public Contains Contains { get; init; } = new();
        public NotContains NotContains { get; init; } = new();

        public EqualTo<int> EqualToInt { get; init; } = new();
        public EqualTo<string> EqualToString { get; init; } = new();

        public NotEqualTo<int> NotEqualToInt { get; init; } = new();
        public NotEqualTo<string> NotEqualToString { get; init; } = new();

        public GreaterThan<int> GreaterThan { get; init; } = new();
        public GreaterThanOrEqual<int> GreaterThanOrEqual { get; init; } = new();

        public LessThan<int> LessThan { get; init; } = new();
        public LessThanOrEqual<int> LessThanOrEqual { get; init; } = new();

        public StartsWith StartsWith { get; init; } = new();
        public EndsWith EndsWith { get; init; } = new();

        public In<int> In { get; init; } = new();
        public NotIn<int> NotIn { get; init; } = new();
    }
}