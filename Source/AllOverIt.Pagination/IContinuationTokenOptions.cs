namespace AllOverIt.Pagination
{
    /// <summary>Provides options that can be applied when serializing a <see cref="ContinuationToken"/>.</summary>
    public interface IContinuationTokenOptions
    {
        /// <summary>Indicates if the serialized continuation token should include an embedded hash code. This defaults to <see cref="ContinuationTokenHashMode.None"/>.</summary>
        ContinuationTokenHashMode HashMode { get; }

        /// <summary>Indicates if the serialized continuation token should be compressed. This is not enabled by default.</summary>
        bool UseCompression { get; }
    }
}
