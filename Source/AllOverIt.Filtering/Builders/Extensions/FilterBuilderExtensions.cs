using AllOverIt.Assertion;
using AllOverIt.Patterns.Specification.Utils;

namespace AllOverIt.Filtering.Builders.Extensions
{
    /// <summary>Provides extension methods for <see cref="IFilterBuilder{TType, TFilter}"/>.</summary>
    public static class FilterBuilderExtensions
    {
        /// <summary>Generates a string representation of the filter specification. This string is intended
        /// only for use in debugging.</summary>
        /// <typeparam name="TType">The object type the specification applies to.</typeparam>
        /// <typeparam name="TFilter">The custom filter type used for defining each operation or comparison in the specification.</typeparam>
        /// <param name="filterBuilder">The filter builder instance.</param>
        /// <returns>A string representation of the filter specification.</returns>
        public static string ToQueryString<TType, TFilter>(this IFilterBuilder<TType, TFilter> filterBuilder)
           where TType : class
           where TFilter : class
        {
            _ = filterBuilder.WhenNotNull(nameof(filterBuilder));

            var visitor = new LinqSpecificationVisitor();

            return visitor.AsQueryString(filterBuilder.AsSpecification);
        }
    }
}