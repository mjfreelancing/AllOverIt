namespace AllOverIt.Pagination
{
    /// <summary>Provides options that can be applied when serializing a <see cref="ContinuationToken"/>.</summary>
    public interface IContinuationTokenOptions
    {
        /// <summary>Indicates if the serialized continuation token should include an embedded hash value. Including a hash value
        /// can be useful for determining if the decoded token will be valid. This is not enabled by default.</summary>
        bool IncludeHash { get; }

        /// <summary>Indicates if the serialized continuation token should be compressed. This is not enabled by default.</summary>
        bool UseCompression { get; }
    }
}
