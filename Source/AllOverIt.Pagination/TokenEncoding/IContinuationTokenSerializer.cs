namespace AllOverIt.Pagination.TokenEncoding
{
    /// <summary>Serializes and deserializes <see cref="IContinuationToken"/> instances.</summary>
    public interface IContinuationTokenSerializer
    {
        /// <summary>Determines if the provided serialized continuation token string is valid.</summary>
        /// <param name="continuationToken">A serialized continuation token.</param>
        /// <returns></returns>
        bool IsValidToken(string continuationToken);

        /// <summary>Serializes a <see cref="IContinuationToken"/> to a string.</summary>
        /// <param name="continuationToken">The <see cref="IContinuationToken"/> to serialize.</param>
        /// <returns>The serialized string value for a <see cref="IContinuationToken"/>.</returns>
        string Serialize(IContinuationToken continuationToken);

        /// <summary>Deserializes a continuation token string to a <see cref="IContinuationToken"/>.</summary>
        /// <param name="continuationToken">The continuation token string to deserialize.</param>
        /// <returns>The <see cref="IContinuationToken"/> deserialized from a provided string value.</returns>
        IContinuationToken Deserialize(string continuationToken);
    }
}
