using System;

namespace AllOverIt.Filtering.Options
{
    /// <inheritdoc cref="IDefaultQueryFilterOptions" />
    public sealed class DefaultQueryFilterOptions : IDefaultQueryFilterOptions
    {
        /// <inheritdoc />
        public bool UseParameterizedQueries { get; init; } = true;

        /// <inheritdoc />
        public StringComparison? StringComparison { get; init; }

        /// <inheritdoc />
        public bool IgnoreNullFilterValues { get; init; }
    }
}