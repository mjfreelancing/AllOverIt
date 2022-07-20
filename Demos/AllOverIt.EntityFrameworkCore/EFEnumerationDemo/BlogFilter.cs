using AllOverIt.Filtering.Filters;

namespace EFEnumerationDemo
{
    public sealed class BlogFilter
    {
        public sealed class IdFilter
        {
            public EqualTo<int> EqualTo { get; set; } = new();
            public NotEqualTo<int> NotEqualTo { get; set; } = new();
            public GreaterThan<int> GreaterThan { get; set; } = new();
            public GreaterThanOrEqual<int> GreaterThanOrEqual { get; set; } = new();
            public LessThan<int> LessThan { get; set; } = new();
            public LessThanOrEqual<int> LessThanOrEqual { get; set; } = new();
            public In<int> In { get; set; } = new();
            public NotIn<int> NotIn { get; set; } = new();
        }

        public sealed class DescriptionFilter
        {
            public EqualTo<string> EqualTo { get; set; } = new();
            public NotEqualTo<string> NotEqualTo { get; set; } = new();
            public Contains Contains { get; set; } = new();
            public NotContains NotContains { get; set; } = new();
            public StartsWith StartsWith { get; set; } = new();
            public EndsWith EndsWith { get; set; } = new();
        }

        public IdFilter Id { get; init; } = new();
        public DescriptionFilter Description { get; init; } = new();
    }
}