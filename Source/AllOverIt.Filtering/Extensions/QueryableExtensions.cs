using AllOverIt.Assertion;
using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Data;
using System.Linq;

namespace AllOverIt.Filtering.Extensions
{
    public interface IFilterSpecificationBuilderOptions
    {
        bool UseParameterizedQueries { get; }
    }

    public sealed class QueryFilterOptions : IFilterSpecificationBuilderOptions
    {
        public bool UseParameterizedQueries { get; init; } = true;
    }

    public static class QueryableExtensions
    {
        public static IQueryable<TType> ApplyFilter<TType, TFilter>(this IQueryable<TType> queryable, TFilter filter,
            Action<IFilterSpecificationBuilder<TType, TFilter>, IFilterBuilder<TType, TFilter>> action, QueryFilterOptions options = default)
            where TType : class
            where TFilter : class, IFilter
        {
            _ = filter.WhenNotNull(nameof(filter));

            // defaults to using parameterized queries
            options ??= new QueryFilterOptions();

            var specificationBuilder = new FilterSpecificationBuilder<TType, TFilter>(filter, options);
            var builder = new FilterBuilder<TType, TFilter>(specificationBuilder);

            action.Invoke(specificationBuilder, builder);

            return queryable.Where(builder.QuerySpecification);
        }
    }
}