using AllOverIt.Extensions;

namespace AllOverIt.Pagination
{
    // NOTE: Unsealed this class so it can be augmented with additional information through inheritance.

    /// <summary>Contains a page of results and token information that can be used to execute a query using an <see cref="IQueryPaginator{TResult}"/>
    /// that obtains the previous or next page of data.</summary>
    /// <typeparam name="TResult"></typeparam>
    public class PageResult<TResult>
    {
        /// <summary>A page of results.</summary>
        public IReadOnlyCollection<TResult> Results { get; init; }

        /// <summary>The total number of records, across all pages, that satisfies the query.</summary>
        public int TotalCount { get; init; }

        /// <summary>The continuation token used to obtain the current page of results.</summary>
        public string CurrentToken { get; init; }

        /// <summary>The continuation token used to obtain the previous page of results relative to the current page of results.
        /// If null, there are no previous pages.</summary>
        public string PreviousToken { get; init; }

        /// <summary>The continuation token used to obtain the next page of results relative to the current page of results.
        /// If null, there are no next pages.</summary>
        public string NextToken { get; init; }

        /// <summary>A factory method to create a <see cref="PageResult{TResult}"/> based on another set of initial (first) results.
        /// This is typically used when one query is used to obtain a page of Id's, for example, and then a secondary query is used
        /// to obtain more detailed information. The more detailed results are returned but the pagination information from the first
        /// query is included.</summary>
        /// <typeparam name="TFirst">The object type associated with <paramref name="firstResults"/>.</typeparam>
        /// <param name="firstResults">The 'first' page of results containing the total record count and continuation tokens.</param>
        /// <param name="results">The page of results to be returned.</param>
        public static PageResult<TResult> CreateFrom<TFirst>(PageResult<TFirst> firstResults, IEnumerable<TResult> results)
        {
            return new PageResult<TResult>
            {
                Results = results.AsReadOnlyCollection(),
                TotalCount = firstResults.TotalCount,
                CurrentToken = firstResults.CurrentToken,
                PreviousToken = firstResults.PreviousToken,
                NextToken = firstResults.NextToken
            };
        }
    }
}