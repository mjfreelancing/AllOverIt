using System.Linq.Expressions;

namespace AllOverIt.Pagination.Extensions
{
    /// <summary>Provides extension methods for <see cref="IQueryPaginator{TEntity}"/>.</summary>
    public static class QueryPaginatorExtensions
    {
        #region ColumnAscending
        /// <summary>Appends multiple columns to be ordered in ascending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnAscending<TEntity, TProp1, TProp2>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2)
            where TEntity : class
        {
            return paginator
                .ColumnAscending(expression1)
                .ColumnAscending(expression2);
        }

        /// <summary>Appends multiple columns to be ordered in ascending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnAscending<TEntity, TProp1, TProp2, TProp3>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3)
            where TEntity : class
        {
            return paginator
                .ColumnAscending(expression1)
                .ColumnAscending(expression2)
                .ColumnAscending(expression3);
        }

        /// <summary>Appends multiple columns to be ordered in ascending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <typeparam name="TProp4">The property type of column 4.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <param name="expression4">The property expression for column 4.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnAscending<TEntity, TProp1, TProp2, TProp3, TProp4>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3,
            Expression<Func<TEntity, TProp4>> expression4)
            where TEntity : class
        {
            return paginator
                .ColumnAscending(expression1)
                .ColumnAscending(expression2)
                .ColumnAscending(expression3)
                .ColumnAscending(expression4);
        }

        /// <summary>Appends multiple columns to be ordered in ascending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <typeparam name="TProp4">The property type of column 4.</typeparam>
        /// <typeparam name="TProp5">The property type of column 5.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <param name="expression4">The property expression for column 4.</param>
        /// <param name="expression5">The property expression for column 5.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnAscending<TEntity, TProp1, TProp2, TProp3, TProp4, TProp5>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3,
            Expression<Func<TEntity, TProp4>> expression4, Expression<Func<TEntity, TProp5>> expression5)
            where TEntity : class
        {
            return paginator
                .ColumnAscending(expression1)
                .ColumnAscending(expression2)
                .ColumnAscending(expression3)
                .ColumnAscending(expression4)
                .ColumnAscending(expression5);
        }

        /// <summary>Appends multiple columns to be ordered in ascending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <typeparam name="TProp4">The property type of column 4.</typeparam>
        /// <typeparam name="TProp5">The property type of column 5.</typeparam>
        /// <typeparam name="TProp6">The property type of column 6.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <param name="expression4">The property expression for column 4.</param>
        /// <param name="expression5">The property expression for column 5.</param>
        /// <param name="expression6">The property expression for column 6.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnAscending<TEntity, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3,
            Expression<Func<TEntity, TProp4>> expression4, Expression<Func<TEntity, TProp5>> expression5, Expression<Func<TEntity, TProp6>> expression6)
            where TEntity : class
        {
            return paginator
                .ColumnAscending(expression1)
                .ColumnAscending(expression2)
                .ColumnAscending(expression3)
                .ColumnAscending(expression4)
                .ColumnAscending(expression5)
                .ColumnAscending(expression6);
        }
        #endregion

        #region ColumnDescending
        /// <summary>Appends multiple columns to be ordered in descending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnDescending<TEntity, TProp1, TProp2>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2)
            where TEntity : class
        {
            return paginator
                .ColumnDescending(expression1)
                .ColumnDescending(expression2);
        }

        /// <summary>Appends multiple columns to be ordered in descending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnDescending<TEntity, TProp1, TProp2, TProp3>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3)
            where TEntity : class
        {
            return paginator
                .ColumnDescending(expression1)
                .ColumnDescending(expression2)
                .ColumnDescending(expression3);
        }

        /// <summary>Appends multiple columns to be ordered in descending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <typeparam name="TProp4">The property type of column 4.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <param name="expression4">The property expression for column 4.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnDescending<TEntity, TProp1, TProp2, TProp3, TProp4>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3,
            Expression<Func<TEntity, TProp4>> expression4)
            where TEntity : class
        {
            return paginator
                .ColumnDescending(expression1)
                .ColumnDescending(expression2)
                .ColumnDescending(expression3)
                .ColumnDescending(expression4);
        }

        /// <summary>Appends multiple columns to be ordered in descending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <typeparam name="TProp4">The property type of column 4.</typeparam>
        /// <typeparam name="TProp5">The property type of column 5.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <param name="expression4">The property expression for column 4.</param>
        /// <param name="expression5">The property expression for column 5.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnDescending<TEntity, TProp1, TProp2, TProp3, TProp4, TProp5>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3,
            Expression<Func<TEntity, TProp4>> expression4, Expression<Func<TEntity, TProp5>> expression5)
            where TEntity : class
        {
            return paginator
                .ColumnDescending(expression1)
                .ColumnDescending(expression2)
                .ColumnDescending(expression3)
                .ColumnDescending(expression4)
                .ColumnDescending(expression5);
        }

        /// <summary>Appends multiple columns to be ordered in descending order. The final specified column must be unique across
        /// all pages, such as a table's IDENTITY column, to ensure pagination behaves as expected.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <typeparam name="TProp1">The property type of column 1.</typeparam>
        /// <typeparam name="TProp2">The property type of column 2.</typeparam>
        /// <typeparam name="TProp3">The property type of column 3.</typeparam>
        /// <typeparam name="TProp4">The property type of column 4.</typeparam>
        /// <typeparam name="TProp5">The property type of column 5.</typeparam>
        /// <typeparam name="TProp6">The property type of column 6.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="expression1">The property expression for column 1.</param>
        /// <param name="expression2">The property expression for column 2.</param>
        /// <param name="expression3">The property expression for column 3.</param>
        /// <param name="expression4">The property expression for column 4.</param>
        /// <param name="expression5">The property expression for column 5.</param>
        /// <param name="expression6">The property expression for column 6.</param>
        /// <returns>The same query paginator instance so a fluent syntax can used to specify additional columns.</returns>
        public static IQueryPaginator<TEntity> ColumnDescending<TEntity, TProp1, TProp2, TProp3, TProp4, TProp5, TProp6>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3,
            Expression<Func<TEntity, TProp4>> expression4, Expression<Func<TEntity, TProp5>> expression5, Expression<Func<TEntity, TProp6>> expression6)
            where TEntity : class
        {
            return paginator
                .ColumnDescending(expression1)
                .ColumnDescending(expression2)
                .ColumnDescending(expression3)
                .ColumnDescending(expression4)
                .ColumnDescending(expression5)
                .ColumnDescending(expression6);
        }
        #endregion

        /// <summary>Executes a paginated query and returns the results along with information pertaining to the previous and next pages of data.</summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="queryPaginator">The paginator containing the configured query to execute.</param>
        /// <param name="continuationToken">The continuation token that describes how to obtain the next (or previous) page of data.</param>
        /// <returns>A page of results along with information about the previous and next page of data available.</returns>
        /// <remarks>This method is only intended for memory based queries. If using EntityFramework Core then it is preferred to use
        /// the GetPageResultsAsync() extension method found in AllOverIt.EntityFrameworkCore.Pagination.</remarks>
        public static PageResult<TResult> GetPageResults<TResult>(this IQueryPaginator<TResult> queryPaginator, string? continuationToken) where TResult : class
        {
            var totalCount = queryPaginator.BaseQuery.Count();
            var pageQuery = queryPaginator.GetPageQuery(continuationToken);
            var pageResults = pageQuery.ToArray();
            var hasResults = pageResults.Length != 0;

            string? previousToken = null;
            string? nextToken = null;

            if (hasResults)
            {
                var (first, last) = queryPaginator.GetQueryDirection() == PaginationDirection.Forward
                    ? (pageResults[0], pageResults[^1])
                    : (pageResults[^1], pageResults[0]);

                var hasPreviousPage = hasResults && queryPaginator.HasPreviousPage(first);

                if (hasPreviousPage)
                {
                    previousToken = queryPaginator.TokenEncoder.EncodePreviousPage(pageResults);
                }

                var hasNextPage = hasResults && queryPaginator.HasNextPage(last);

                if (hasNextPage)
                {
                    nextToken = queryPaginator.TokenEncoder.EncodeNextPage(pageResults);
                }
            }

            return new PageResult<TResult>
            {
                Results = pageResults,
                TotalCount = totalCount,
                CurrentToken = continuationToken,
                PreviousToken = previousToken,
                NextToken = nextToken
            };
        }
    }
}
