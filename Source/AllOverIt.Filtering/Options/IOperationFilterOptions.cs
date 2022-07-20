using System;

namespace AllOverIt.Filtering.Options
{
    public interface IOperationFilterOptions
    {
        bool UseParameterizedQueries { get; }               // Only set, if required, for non-database queries
        StringComparison? StringComparison { get; }         // Only set, if required, for non-database queries
        bool IgnoreNullFilterValue { get; }
    }
}