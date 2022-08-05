namespace AllOverIt.Filtering.Options
{
    /// <inheritdoc cref="IOperationFilterOptions" />
    public sealed class OperationFilterOptions : IOperationFilterOptions
    {
        /// <inheritdoc />
        public bool UseParameterizedQueries { get; set; }

        /// <inheritdoc />
        public StringComparisonMode StringComparisonMode { get; init; } = StringComparisonMode.None;

        /// <inheritdoc />
        public bool IgnoreNullFilterValue { get; set; }
    }
}