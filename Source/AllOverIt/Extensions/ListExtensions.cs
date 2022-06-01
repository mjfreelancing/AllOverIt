using AllOverIt.Assertion;
using System.Collections.Generic;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IReadOnlyList{TType}"/> and <see cref="IList{TType}"/> types.</summary>
    public static class ListExtensions
    {
        /// <summary>An alternative to the LINQ First() method that efficiently returns the first element of a <see cref="IReadOnlyList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The collection of items.</param>
        /// <returns>The first element.</returns>
        public static TType FirstElement<TType>(this IReadOnlyList<TType> items)
        {
            _ = items.WhenNotNullOrEmpty(nameof(items));

            return items[0];
        }

        /// <summary>An alternative to the LINQ Last() method that efficiently returns the last element of a <see cref="IReadOnlyList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The collection of items.</param>
        /// <returns>The last element.</returns>
        public static TType LastElement<TType>(this IReadOnlyList<TType> items)
        {
            _ = items.WhenNotNullOrEmpty(nameof(items));

            return items[items.Count - 1];
        }

        /// <summary>An alternative to the LINQ First() method that efficiently returns the first element of a <see cref="IList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The collection of items.</param>
        /// <returns>The first element.</returns>
        public static TType FirstElement<TType>(this IList<TType> items)
        {
            _ = items.WhenNotNullOrEmpty(nameof(items));

            return items[0];
        }

        /// <summary>An alternative to the LINQ Last() method that efficiently returns the last element of a <see cref="IList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The collection of items.</param>
        /// <returns>The last element.</returns>
        public static TType LastElement<TType>(this IList<TType> items)
        {
            _ = items.WhenNotNullOrEmpty(nameof(items));

            return items[items.Count - 1];
        }
    }
}