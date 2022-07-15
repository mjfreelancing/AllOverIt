using AllOverIt.Assertion;
using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Data;
using System.Linq;

namespace AllOverIt.Filtering.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TType> ApplyFilter<TType, TFilter>(this IQueryable<TType> queryable, TFilter filter,
            Action<IFilterSpecificationBuilder<TType, TFilter>, IFilterBuilder<TType, TFilter>> action)
            where TType : class
            where TFilter : class, IFilter
        {
            _ = filter.WhenNotNull(nameof(filter));

            var specificationBuilder = new FilterSpecificationBuilder<TType, TFilter>(filter);
            var builder = new FilterBuilder<TType, TFilter>(specificationBuilder);

            action.Invoke(specificationBuilder, builder);

            return queryable.Where(builder.QuerySpecification);
        }
    }
}