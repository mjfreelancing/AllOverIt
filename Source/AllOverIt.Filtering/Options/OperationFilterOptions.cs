using System;

namespace AllOverIt.Filtering.Options
{
    public sealed class OperationFilterOptions : IOperationFilterOptions
    {
        public bool UseParameterizedQueries { get; set; }
        public StringComparison? StringComparison { get; set; }        // Only set, if required, for non-database queries
        public bool IgnoreNullFilterValue { get; set; }
    }
}