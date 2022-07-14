using AllOverIt.Extensions;
using AllOverIt.Filtering.Operations;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> ContainsSpecification<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, string>> propertyExpression, string value)
            where TEntity : class
        {
            var contains = new Contains<TEntity>(propertyExpression, value);

            return queryable.Where(contains.Expression);
        }



        public static IQueryable<TEntity> InSpecification<TEntity, TProperty>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, TProperty>> propertyExpression,
            IList<TProperty> values)
            where TEntity : class
        {
            var contains = new In<TEntity, TProperty>(propertyExpression, values);

            return queryable.Where(contains.Expression);
        }



        public static ILinqSpecification<TEntity> OrSpecification<TEntity, TProperty>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, TProperty>> propertyExpression,
            IList<TProperty> values)
            where TEntity : class
        {
            return new In<TEntity, TProperty>(propertyExpression, values);

            //return queryable.Where(contains.Expression);
        }

    }
}