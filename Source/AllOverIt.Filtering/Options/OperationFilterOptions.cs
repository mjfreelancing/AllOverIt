using AllOverIt.Expressions.Strings;

namespace AllOverIt.Filtering.Options
{
    /// <inheritdoc cref="IOperationFilterOptions" />
    public sealed class OperationFilterOptions : IOperationFilterOptions
    {
        /// <inheritdoc />
        public bool UseParameterizedQueries { get; set; } = true;

        /// <inheritdoc />
        public StringComparisonMode StringComparisonMode { get; init; } = StringComparisonMode.None;

        /// <inheritdoc />
        public bool IgnoreDefaultFilterValue { get; set; } = true;
    }
}