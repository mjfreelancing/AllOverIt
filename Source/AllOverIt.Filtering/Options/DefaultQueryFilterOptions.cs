using AllOverIt.Expressions.Strings;

namespace AllOverIt.Filtering.Options
{
    /// <inheritdoc cref="IDefaultQueryFilterOptions" />
    public sealed class DefaultQueryFilterOptions : IDefaultQueryFilterOptions
    {
        /// <inheritdoc />
        public bool UseParameterizedQueries { get; init; } = true;

        /// <inheritdoc />
        public StringComparisonMode StringComparisonMode { get; init; } = StringComparisonMode.None;

        /// <inheritdoc />
        public bool IgnoreNullFilterValues { get; init; }
    }
}