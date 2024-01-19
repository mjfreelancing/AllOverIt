namespace AllOverIt.Pagination.TokenEncoding
{
    /// <summary>Represents a serialized continuation token validator.</summary>
    public interface IContinuationTokenValidator
    {
        /// <summary>Validates a serialized continuation token using specified options.</summary>
        /// <param name="continuationToken">The serialized continuation token to be validated.</param>
        /// <param name="tokenOptions">The serializer options that were used to create the serialized continuation token.</param>
        /// <returns><see langword="true" /> if the continuation token is valid, otherwise <see langword="false" />.</returns>
        bool IsValidToken(string continuationToken, IContinuationTokenOptions tokenOptions);
    }
}
