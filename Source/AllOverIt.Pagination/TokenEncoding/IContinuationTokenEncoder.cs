namespace AllOverIt.Pagination.TokenEncoding
{
    /// <summary>Describes a token generator that simplifies requesting next and previous page queries.</summary>
    public interface IContinuationTokenEncoder
    {
        /// <summary>The serializer responsible for encoding continuation tokens.</summary>
        IContinuationTokenSerializer Serializer { get; }

        /// <summary>Creates a continuation token encoded with information that indicates the required pagination direction and
        /// the column values from a reference entity that identify the record immediately adjacent to the requiredprevious page.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <param name="references">Typically the rows returned from a previously queried page. This method selects the first or last
        /// record as a reference entity based on the direction that defines the 'previous' page relative to the query's pagination
        /// direction.</param>
        /// <returns>An encoded continuation token that can be used to query the previous page, relative to a reference entity and
        /// the query's pagination direction.</returns>
        string EncodePreviousPage<TEntity>(IReadOnlyCollection<TEntity> references) where TEntity : class;

        /// <summary>Creates a continuation token encoded with information that indicates the required pagination direction and
        /// the column values from a reference entity that identify the record immediately adjacent to the required next page.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <param name="references">Typically the rows returned from a previously queried page. This method selects the first or last
        /// record as a reference entity based on the direction that defines the 'next' page relative to the query's pagination
        /// direction.</param>
        /// <returns>An encoded continuation token that can be used to query the next page, relative to a reference entity and
        /// the query's pagination direction.</returns>
        string EncodeNextPage<TEntity>(IReadOnlyCollection<TEntity> references) where TEntity : class;

        /// <summary>Creates a continuation token encoded with information that indicates the required pagination direction and
        /// the column values from the provided reference entity.</summary>
        /// <param name="reference">A reference entity, or an anonymous object with the required column values, that is immediately
        /// adjacent to the required previous page.</param>
        /// <returns>An encoded continuation token that can be used to query the previous page, relative to a reference entity and
        /// the query's pagination direction.</returns>
        //
        // Note: Cannot use <TEntity> because the compiler may choose this overload when it should choose the
        //       IReadOnlyCollection<TEntity> version (when TEntity is an anonymous object and the caller
        //       does not (cannot) explicitly declare using <TEntity>.
        string EncodePreviousPage(object reference);

        /// <summary>Creates a continuation token encoded with information that indicates the required pagination direction and
        /// the column values from the provided reference entity.</summary>
        /// <param name="reference">A reference entity, or an anonymous object with the required column values, that is immediately
        /// adjacent to the required next page.</param>
        /// <returns>An encoded continuation token that can be used to query the next page, relative to a reference entity and
        /// the query's pagination direction.</returns>
        //
        // Note: Cannot use <TEntity> because the compiler may choose this overload when it should choose the
        //       IReadOnlyCollection<TEntity> version (when TEntity is an anonymous object and the caller
        //       does not (cannot) explicitly declare using <TEntity>.
        string EncodeNextPage(object reference);

        /// <summary>Creates a continuation that can be used to obtain the first page of a query.</summary>
        string EncodeFirstPage();

        /// <summary>Creates a continuation that can be used to obtain the last page of a query.</summary>
        string EncodeLastPage();
    }
}
