namespace AllOverIt.Pagination
{
    /// <summary>Provides options that can be applied when serializing a <see cref="ContinuationToken"/>.</summary>
    public sealed class ContinuationTokenOptions : IContinuationTokenOptions
    {
        /// <summary>A default <see cref="ContinuationTokenOptions"/> instance.</summary>
        public static readonly ContinuationTokenOptions Default = new();

        /// <inheritdoc />
        public bool IncludeHash { get; set; }

        /// <inheritdoc />
        public bool UseCompression { get; set; }
    }
}
