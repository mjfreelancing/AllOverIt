using AllOverIt.Filtering.Filters;

namespace EFEnumerationDemo
{
    public sealed class BlogFilter : IFilter
    {
        public sealed class IdFilter
        {
            public EqualTo<int> EqualTo { get; set; }
            public NotEqualTo<int> NotEqualTo { get; set; }
            public GreaterThan<int> GreaterThan { get; set; }
            public GreaterThanOrEqual<int> GreaterThanOrEqual { get; set; }
            public LessThan<int> LessThan { get; set; }
            public LessThanOrEqual<int> LessThanOrEqual { get; set; }
            public In<int> In { get; set; }
            public NotIn<int> NotIn { get; set; }
        }

        public sealed class DescriptionFilter
        {
            public EqualTo<string> EqualTo { get; set; }
            public NotEqualTo<string> NotEqualTo { get; set; }
            public Contains Contains { get; set; }
            public NotContains NotContains { get; set; }
            public StartsWith StartsWith { get; set; }
            public EndsWith EndsWith { get; set; }
        }

        public IdFilter Id { get; init; } = new();
        public DescriptionFilter Description { get; init; } = new();
    }
}