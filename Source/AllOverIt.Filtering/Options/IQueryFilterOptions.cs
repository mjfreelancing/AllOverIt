using System;

namespace AllOverIt.Filtering.Options
{
    public interface IQueryFilterOptions
    {
        bool UseParameterizedQueries { get; }            // Only set, if required, for non-database queries
        StringComparison? StringComparison { get; }      // Only set, if required, for non-database queries
        bool IgnoreNullFilterValues { get; }
    }
}