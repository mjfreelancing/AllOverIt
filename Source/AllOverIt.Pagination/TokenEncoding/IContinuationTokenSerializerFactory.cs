namespace AllOverIt.Pagination.TokenEncoding
{
    /// <summary>Represents factory that creates <see cref="IContinuationTokenSerializer"/> instances.</summary>
    public interface IContinuationTokenSerializerFactory
    {
        /// <summary>Creates a new <see cref="IContinuationTokenSerializer"/> instance.</summary>
        /// <param name="continuationTokenOptions">The token options to be used by the serializer.</param>
        /// <returns>A new <see cref="IContinuationTokenSerializer"/> instance.</returns>
        IContinuationTokenSerializer CreateContinuationTokenSerializer(IContinuationTokenOptions continuationTokenOptions);
    }
}
