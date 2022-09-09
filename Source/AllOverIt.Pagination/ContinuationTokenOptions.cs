namespace AllOverIt.Pagination
{
    /// <summary>Provides options that can be applied when serializing a <see cref="ContinuationToken"/>.</summary>
    public sealed class ContinuationTokenOptions : IContinuationTokenOptions
    {
        public static readonly ContinuationTokenOptions None = new();

        /// <inheritdoc />
        public ContinuationTokenHashMode HashMode { get; set; } = ContinuationTokenHashMode.None;

        /// <inheritdoc />
        public bool UseCompression { get; set; }
    }
}
