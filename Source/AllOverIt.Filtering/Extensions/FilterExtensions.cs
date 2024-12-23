﻿using AllOverIt.Assertion;
using AllOverIt.Filtering.Filters;

namespace AllOverIt.Filtering.Extensions
{
    /// <summary>Provides extension methods for various filter operations.</summary>
    public static class FilterExtensions
    {
        /// <summary>Indicates if the filter has a non-null value with at least one element.</summary>
        /// <typeparam name="TType">The filter element type.</typeparam>
        /// <param name="filter">The filter instance.</param>
        /// <returns><see langword="True" /> if the filter has a non-null value with at least one element, otherwise <see langword="False" />.</returns>
        public static bool HasValue<TType>(this IArrayFilterOperation<TType> filter)
        {
            _ = filter.WhenNotNull();

            return filter.Value?.Any() ?? false;
        }

        /// <summary>Indicates if the filter has a non-null value.</summary>
        /// <param name="filter">The filter instance.</param>
        /// <returns><see langword="True" /> if the filter has a non-null value, otherwise <see langword="False" />.</returns>
        public static bool HasValue(this IStringFilterOperation filter)
        {
            _ = filter.WhenNotNull();

            return filter.Value is not null;
        }

        /// <summary>Indicates if the filter has a non-null value.</summary>
        /// <typeparam name="TType">The (nullable) filter type.</typeparam>
        /// <param name="filter">The filter instance.</param>
        /// <returns><see langword="True" /> if the filter has a non-null value, otherwise <see langword="False" />.</returns>
        public static bool HasValue<TType>(this IBasicFilterOperation<TType?> filter) where TType : struct
        {
            _ = filter.WhenNotNull();

            return filter.Value.HasValue;
        }
    }
}