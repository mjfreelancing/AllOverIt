using AllOverIt.Filtering.Filters;
using System;

namespace FilteringDemo
{
    public sealed class ProductFilter : IFilter
    {
        public sealed class ActiveFilter
        {
            public EqualTo<bool> EqualTo { get; set; } 
        }

        public sealed class CategoryFilter
        {
            public StartsWith StartsWith { get; set; }
        }

        public sealed class NameFilter
        {
            public Contains Contains { get; set; }
        }

        public sealed class PriceFilter
        {
            public GreaterThanOrEqual<double> GreaterThanOrEqual { get; set; }
            public LessThanOrEqual<double> LessThanOrEqual { get; set; }
        }

        public sealed class LastUpdatedFilter
        {
            public GreaterThanOrEqual<DateTime> GreaterThanOrEqual { get; set; }
        }

        public ActiveFilter Active { get; init; } = new();
        public CategoryFilter Category { get; init; } = new();
        public NameFilter Name { get; init; } = new();
        public PriceFilter Price { get; init; } = new();
        public LastUpdatedFilter LastUpdated { get; init; } = new();
    }
}