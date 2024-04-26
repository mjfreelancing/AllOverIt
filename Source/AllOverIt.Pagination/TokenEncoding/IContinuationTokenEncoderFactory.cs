namespace AllOverIt.Pagination.TokenEncoding
{
    /// <summary>Represents a factory for creating continuation token encoders.</summary>
    public interface IContinuationTokenEncoderFactory
    {
        /// <summary>Creates a <see cref="IContinuationTokenEncoder"/> instance for a specified set of column definitions, pagination direction and
        /// options that control how the token will be serialized.</summary>
        /// <param name="columns">The column definitions to be encoded.</param>
        /// <param name="paginationDirection">The pagination direction to be encoded.</param>
        /// <param name="continuationTokenOptions">Options that control how the encoded token will be serialized.</param>
        /// <returns>A <see cref="IContinuationTokenEncoder"/> instance specific to the provided column definitions and pagination direction.</returns>
        IContinuationTokenEncoder CreateContinuationTokenEncoder(IReadOnlyCollection<IColumnDefinition> columns, PaginationDirection paginationDirection,
            ContinuationTokenOptions continuationTokenOptions);
    }
}
