namespace AllOverIt.Pagination
{
    /// <summary>Specifies a hash mode to use when serializing a <see cref="ContinuationToken"/>.</summary>
    public enum ContinuationTokenHashMode
    {
        /// <summary>Do not encode a hash value within the serialized <see cref="ContinuationToken"/>.</summary>
        None,

        /// <summary>Encodes a SHA1 hash value (160 bits) within the serialized <see cref="ContinuationToken"/>. Using this mode
        /// will increase the final token length by 28 characters.</summary>
        Sha1,

        /// <summary>Encodes a SHA256 hash value (256 bits) within the serialized <see cref="ContinuationToken"/>. Using this mode
        /// will increase the final token length by 44 characters.</summary>
        Sha256,

        /// <summary>Encodes a SHA256 hash value (384 bits) within the serialized <see cref="ContinuationToken"/>. Using this mode
        /// will increase the final token length by 64 characters.</summary>
        Sha384,

        /// <summary>Encodes a SHA256 hash value (512 bits) within the serialized <see cref="ContinuationToken"/>. Using this mode
        /// will increase the final token length by 88 characters.</summary>
        Sha512
    }
}
