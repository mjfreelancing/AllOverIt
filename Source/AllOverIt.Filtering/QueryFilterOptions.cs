using AllOverIt.Filtering.Builders;
using System;

namespace AllOverIt.Filtering
{
    public sealed class QueryFilterOptions : IFilterSpecificationBuilderOptions
    {
        public bool UseParameterizedQueries { get; init; } = true;
        public StringComparison? StringComparison { get; init; }        // Only set, if required, for non-database queries
    }
}