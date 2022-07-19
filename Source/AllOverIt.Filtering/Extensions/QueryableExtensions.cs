using AllOverIt.Assertion;
using AllOverIt.Filtering.Builders;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Data;
using System.Linq;

namespace AllOverIt.Filtering.Extensions
{
    public interface IFilterSpecificationBuilderOptions
    {
        bool UseParameterizedQueries { get; }
        StringComparison? StringComparison { get; }
    }

    public sealed class QueryFilterOptions : IFilterSpecificationBuilderOptions
    {
        public bool UseParameterizedQueries { get; init; } = true;
        public StringComparison? StringComparison { get; init; }        // Only set, if required, for non-database queries
    }

    public static class QueryableExtensions
    {
        public static IQueryable<TType> ApplyFilter<TType, TFilter>(this IQueryable<TType> queryable, TFilter filter,
            Action<IFilterSpecificationBuilder<TType, TFilter>, IFilterBuilder<TType, TFilter>> action, QueryFilterOptions options = default)
            where TType : class
            where TFilter : class
        {
            _ = filter.WhenNotNull(nameof(filter));

            // defaults to using parameterized queries
            options ??= new QueryFilterOptions();

            var specificationBuilder = new FilterSpecificationBuilder<TType, TFilter>(filter, options);
            var builder = new FilterBuilder<TType, TFilter>(specificationBuilder);

            action.Invoke(specificationBuilder, builder);

            return queryable.Where(builder.AsSpecification);
        }
    }
}