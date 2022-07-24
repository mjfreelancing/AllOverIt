using System;

namespace AllOverIt.Filtering.Options
{
    /// <inheritdoc cref="IOperationFilterOptions" />
    public sealed class OperationFilterOptions : IOperationFilterOptions
    {
        /// <inheritdoc />
        public bool UseParameterizedQueries { get; set; }

        /// <inheritdoc />
        public StringComparison? StringComparison { get; set; }

        /// <inheritdoc />
        public bool IgnoreNullFilterValue { get; set; }
    }
}