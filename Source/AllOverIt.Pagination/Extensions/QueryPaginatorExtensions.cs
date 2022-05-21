using System;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Pagination.Extensions
{
    public static class QueryPaginatorExtensions
    {
        public static IQueryable<TEntity> BuildFirstPageQuery<TEntity>(this IQueryPaginator<TEntity> paginator)
            where TEntity : class
        {
            return paginator.BuildForwardPageQuery(null);
        }

        public static IQueryable<TEntity> BuildLastPageQuery<TEntity>(this IQueryPaginator<TEntity> paginator)
            where TEntity : class
        {
            return paginator.BuildBackPageQuery(null);
        }

        // todo: Build async method in EF
        public static bool HasPreviousPage<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference)
            where TEntity : class
        {
            return paginator.BuildBackPageQuery(reference).Any();
        }

        // todo: Build async method in EF
        public static bool HasNextPage<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference)
            where TEntity : class
        {
            return paginator.BuildForwardPageQuery(reference).Any();
        }

        #region ColumnAscending
        public static IQueryPaginator<TEntity> ColumnAscending<TEntity, TProp1, TProp2>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2)
            where TEntity : class
        {
            return paginator
                .ColumnAscending(expression1)
                .ColumnAscending(expression2);
        }

        public static IQueryPaginator<TEntity> ColumnAscending<TEntity, TProp1, TProp2, TProp3>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3)
            where TEntity : class
        {
            return paginator
                .ColumnAscending(expression1)
                .ColumnAscending(expression2)
                .ColumnAscending(expression3);
        }

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
        public static IQueryPaginator<TEntity> ColumnDescending<TEntity, TProp1, TProp2>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2)
            where TEntity : class
        {
            return paginator
                .ColumnDescending(expression1)
                .ColumnDescending(expression2);
        }

        public static IQueryPaginator<TEntity> ColumnDescending<TEntity, TProp1, TProp2, TProp3>(this IQueryPaginator<TEntity> paginator,
            Expression<Func<TEntity, TProp1>> expression1, Expression<Func<TEntity, TProp2>> expression2, Expression<Func<TEntity, TProp3>> expression3)
            where TEntity : class
        {
            return paginator
                .ColumnDescending(expression1)
                .ColumnDescending(expression2)
                .ColumnDescending(expression3);
        }

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
    }
}
