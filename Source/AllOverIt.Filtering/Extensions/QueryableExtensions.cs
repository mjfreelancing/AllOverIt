using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification.Extensions;
using System.Data;

namespace AllOverIt.Filtering.Extensions
{
    /// <summary>Provides extension methods for <see cref="IQueryable{TType}"/>.</summary>
    public static class QueryableExtensions
    {
        /// <summary>Provides the ability to apply a custom filter to an <see cref="IQueryable"/>. The <paramref name="action"/>
        /// provides a specification builder that can be used to build complex rules (specifications) that can then be applied via
        /// the filter builder. The filter builder can also be used to build more generic AND and OR predicates. The predicates added
        /// to the filter builder are converted to specifications that are then invokable by the <see cref="IQueryable"/>.</summary>
        /// <typeparam name="TType">The object type the specifications are applied to.</typeparam>
        /// <typeparam name="TFilter">The custom filter type used for defining each operation or comparison in the final specifications</typeparam>
        /// <param name="queryable">The <see cref="IQueryable"/> to apply the filter to.</param>
        /// <param name="filter">A filter context that is provided to the <paramref name="action"/> so it can be used to determine how
        /// to build the filter.</param>
        /// <param name="action">Provides a specification builder that can be used to build complex specifications (predicates). If used,
        /// these specifications can then be added to the also provided filter builder. The filter builder is used to add one or more
        /// predicates (criteria) to the <paramref name="queryable"/>.</param>
        /// <param name="options">Provides options that control how predicates are constructed.</param>
        /// <returns>An updated <paramref name="queryable"/> that includes all predicates or specifications that were added via the filter
        /// builder.</returns>
        public static IQueryable<TType> ApplyFilter<TType, TFilter>(this IQueryable<TType> queryable, TFilter filter,
            Action<IFilterSpecificationBuilder<TType, TFilter>, IFilterBuilder<TType, TFilter>> action, DefaultQueryFilterOptions? options = default)
            where TType : class
            where TFilter : class
        {
            _ = filter.WhenNotNull(nameof(filter));

            // defaults to using parameterized queries as this is mostly likely to be used with EF (or similar) queries
            options ??= new DefaultQueryFilterOptions();

            var specificationBuilder = new FilterSpecificationBuilder<TType, TFilter>(filter, options);
            var builder = new FilterBuilder<TType, TFilter>(specificationBuilder);

            action.Invoke(specificationBuilder, builder);

            var specification = builder.AsSpecification();

            return queryable.Where(specification);
        }
    }
}