using AllOverIt.Extensions;
using AllOverIt.Filtering.Filters;
using System.Linq;

namespace AllOverIt.Filtering.Builders.Extensions
{
    /// <summary>Provides extension methods for various filter operations.</summary>
    public static class FilterExtensions
    {
        /// <summary>Indicates if the filter has a non-null value with at least one element.</summary>
        /// <typeparam name="TType">The filter element type.</typeparam>
        /// <param name="filter">The filter instance.</param>
        /// <returns>True if the filter has a non-null value with at least one element, otherwise false.</returns>
        public static bool Any<TType>(this IArrayFilterOperation<TType> filter)
        {
            return filter.Value?.Any() ?? false;
        }

        /// <summary>Indicates if the filter has a non-null value that is not empty nor contains whitespace.</summary>
        /// <param name="filter">The filter instance.</param>
        /// <returns>True if the filter has a non-null value that is not empty nor contains whitespace, otherwise false.</returns>
        public static bool Any(this IStringFilterOperation filter)
        {
            return filter.Value.IsNotNullOrEmpty();
        }

        /// <summary>Indicates if the filter has a non-null value.</summary>
        /// <typeparam name="TType">The (nullable) filter type.</typeparam>
        /// <param name="filter">The filter instance.</param>
        /// <returns>True if the filter has a non-null value, otherwise false.</returns>
        public static bool Any<TType>(this IBasicFilterOperation<TType?> filter) where TType : struct
        {
            return filter.Value.HasValue;
        }
    }
}