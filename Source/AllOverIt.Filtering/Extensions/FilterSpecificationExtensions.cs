using AllOverIt.Assertion;
using AllOverIt.Filtering.Builders;
using AllOverIt.Patterns.Specification.Utils;

namespace AllOverIt.Filtering.Extensions
{
    /// <summary>Provides extension methods for <see cref="IFilterBuilder{TType, TFilter}"/>.</summary>
    public static class FilterSpecificationExtensions
    {
        /// <summary>Generates a string representation of the filter specification. This string is intended
        /// only for use in debugging.</summary>
        /// <typeparam name="TType">The object type the specification applies to.</typeparam>
        /// <typeparam name="TFilter">The custom filter type used for defining each operation or comparison in the specification.</typeparam>
        /// <param name="filterBuilder">The filter builder instance.</param>
        /// <param name="visitor">An optional visitor that can resolve node values for custom types.</param>
        /// <returns>A string representation of the filter specification.</returns>
        public static string ToQueryString<TType, TFilter>(this IFilterSpecification<TType, TFilter> filterBuilder, LinqSpecificationVisitor visitor = default)
           where TType : class
           where TFilter : class
        {
            _ = filterBuilder.WhenNotNull(nameof(filterBuilder));

            visitor ??= new LinqSpecificationVisitor();

            return visitor.AsQueryString(filterBuilder.AsSpecification());
        }
    }
}