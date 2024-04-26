using AllOverIt.Assertion;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IReadOnlyList{TType}"/> and <see cref="IList{TType}"/> types.</summary>
    public static class ListExtensions
    {
        /// <summary>An alternative to the LINQ First() method that efficiently returns the first element of a <see cref="IReadOnlyList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="list">The source list.</param>
        /// <returns>The first element.</returns>
        public static TType FirstElement<TType>(this IReadOnlyList<TType> list)
        {
            _ = list.WhenNotNullOrEmpty(nameof(list));

            return list[0];
        }

        /// <summary>An alternative to the LINQ Last() method that efficiently returns the last element of a <see cref="IReadOnlyList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="list">The source list.</param>
        /// <returns>The last element.</returns>
        public static TType LastElement<TType>(this IReadOnlyList<TType> list)
        {
            _ = list.WhenNotNullOrEmpty(nameof(list));

            return list[list.Count - 1];
        }

        /// <summary>An alternative to the LINQ First() method that efficiently returns the first element of a <see cref="IList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="list">The source list.</param>
        /// <returns>The first element.</returns>
        public static TType FirstElement<TType>(this IList<TType> list)
        {
            _ = list.WhenNotNullOrEmpty(nameof(list));

            return list[0];
        }

        /// <summary>An alternative to the LINQ Last() method that efficiently returns the last element of a <see cref="IList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="list">The source list.</param>
        /// <returns>The last element.</returns>
        public static TType LastElement<TType>(this IList<TType> list)
        {
            _ = list.WhenNotNullOrEmpty(nameof(list));

            return list[list.Count - 1];
        }

        /// <summary>Appends a collection of elements to an existing <see cref="IList{TType}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="list">The source list.</param>
        /// <param name="items">The collection of elements to append to the source list.</param>
        public static void AddMany<TType>(this IList<TType> list, IEnumerable<TType> items)
        {
            _ = list.WhenNotNull(nameof(list));
            _ = items.WhenNotNull(nameof(items));

            if (list is List<TType> sourceList)
            {
                // Will be more efficient if an actual List<TType>
                sourceList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }
    }
}