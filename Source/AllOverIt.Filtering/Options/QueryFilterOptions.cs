using System;

namespace AllOverIt.Filtering.Options
{
    public sealed class QueryFilterOptions : IQueryFilterOptions
    {
        public bool UseParameterizedQueries { get; init; } = true;
        public StringComparison? StringComparison { get; init; }
        public bool IgnoreNullFilterValues { get; init; }
    }
}