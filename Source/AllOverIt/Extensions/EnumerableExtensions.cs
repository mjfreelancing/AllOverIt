using AllOverIt.Assertion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IEnumerable{T}"/>.</summary>
    public static partial class EnumerableExtensions
    {
        /// <summary>Indicates if the source items contains no elements.</summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of elements.</param>
        /// <returns>True if the source items contains no elements, otherwise false.</returns>
        public static bool NotAny<TType>(this IEnumerable<TType> items)
        {
            _ = items.WhenNotNull(nameof(items));

            return !items.Any();
        }

        /// <summary>Returns the source items as an IList.</summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be returned as an IList.</param>
        /// <returns>If the source items is already an IList. If the source is already an IList then the same reference is returned,
        /// otherwise a new list is created and populated before being returned.</returns>
        public static IList<TType> AsList<TType>(this IEnumerable<TType> items)
        {
            _ = items.WhenNotNull(nameof(items));

            return items as IList<TType> ?? items.ToList();
        }

        /// <summary>Returns the source items as an IReadOnlyList.</summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be returned as an IReadOnlyList.</param>
        /// <returns>The source items as an IReadOnlyList. If the source is already an IReadOnlyList then the same
        /// reference is returned, otherwise a new list is created and populated before being returned.</returns>
        public static IReadOnlyList<TType> AsReadOnlyList<TType>(this IEnumerable<TType> items)
        {
            _ = items.WhenNotNull(nameof(items));

            return items as IReadOnlyList<TType> ?? items.ToList();
        }

        /// <summary>Returns the source items as an IReadOnlyCollection.</summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be returned as an IReadOnlyCollection.</param>
        /// <returns>The source items as an IReadOnlyCollection. If the source is already an IReadOnlyCollection then the same
        /// reference is returned, otherwise a new list is created and populated before being returned.</returns>
        public static IReadOnlyCollection<TType> AsReadOnlyCollection<TType>(this IEnumerable<TType> items)
        {
            _ = items.WhenNotNull(nameof(items));

            return items as IReadOnlyCollection<TType> ?? new ReadOnlyCollection<TType>(items.AsList());
        }

#if !NETSTANDARD2_0
        /// <summary>Asynchronously projects each item within a sequence.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An enumerator that provides asynchronous iteration over a sequence of elements.</returns>
        public static async IAsyncEnumerable<TResult> SelectAsync<TType, TResult>(this IEnumerable<TType> items, Func<TType, Task<TResult>> selector,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return await selector.Invoke(item).ConfigureAwait(false);
            }
        }
#endif

        /// <summary>Projects each element into another form and returns the result as an IList{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IList{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <returns>The projected results as an IList{TResult}.</returns>
        public static IList<TResult> SelectAsList<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selector)
        {
            _ = items.WhenNotNull(nameof(items));

            return items.Select(selector).ToList();
        }

        /// <summary>Projects each element into another form and returns the result as an IReadOnlyCollection{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyCollection{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <returns>The projected results as an IReadOnlyCollection{TResult}.</returns>
        public static IReadOnlyCollection<TResult> SelectAsReadOnlyCollection<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selector)
        {
            _ = items.WhenNotNull(nameof(items));

            return items.Select(selector).ToList();
        }

        /// <summary>Projects each element into another form and returns the result as an IReadOnlyList{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyList{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <returns>The projected results as an IReadOnlyList{TResult}.</returns>
        public static IReadOnlyList<TResult> SelectAsReadOnlyList<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selector)
        {
            _ = items.WhenNotNull(nameof(items));

            return items.Select(selector).ToList();
        }

#if !NETSTANDARD2_0
        /// <summary>Asynchronously projects each element into another form and returns the result as an IReadOnlyCollection{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyCollection{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A CancellationToken to cancel the operation.</param>
        /// <returns>The projected results as an IReadOnlyCollection{TResult}.</returns>
        public static async Task<IReadOnlyCollection<TResult>> SelectAsReadOnlyCollectionAsync<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, Task<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var results = await items
                .SelectAsync(selector, cancellationToken)
                .ToListAsync(cancellationToken);

            return results.AsReadOnlyCollection();
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as an IReadOnlyList{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyList{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A CancellationToken to cancel the operation.</param>
        /// <returns>The projected results as an IReadOnlyList{TResult}.</returns>
        public static async Task<IReadOnlyList<TResult>> SelectAsReadOnlyListAsync<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, Task<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var results = await items
                .SelectAsync(selector, cancellationToken)
                .ToListAsync(cancellationToken);

            return results.AsReadOnlyList();
        }
#endif

        /// <summary>
        /// Applicable to strings and collections, this method determines if the instance is null or empty.
        /// </summary>
        /// <param name="items">The object of interest.</param>
        /// <returns>True if the object is null or empty.</returns>
        public static bool IsNullOrEmpty(this IEnumerable items)
        {
            return items == null || !items.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Partitions a collection into multiple batches of a maximum size. 
        /// </summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be partitioned.</param>
        /// <param name="batchSize">The maximum number of items in each batch.</param>
        /// <returns>One or more batches containing the source items partitioned into a maximum batch size.</returns>
        public static IEnumerable<IEnumerable<TType>> Batch<TType>(this IEnumerable<TType> items, int batchSize)
        {
            _ = items.WhenNotNull(nameof(items));

            var batch = new List<TType>(batchSize);

            foreach (var item in items)
            {
                batch.Add(item);

                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<TType>(batchSize);
                }
            }

            if (batch.Any())
            {
                yield return batch;
            }
        }

        /// <summary>Projects each element of a sequence into a new form that includes the element's zero-based index.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The source sequence of elements.</param>
        /// <returns>The projected sequence including the element's index.</returns>
        public static IEnumerable<(TType Item, int Index)> WithIndex<TType>(this IEnumerable<TType> items)
        {
            _ = items.WhenNotNull(nameof(items));

            return items.Select((item, index) => (item, index));
        }

        /// <summary>Iterates a sequence of elements and provides the zero-based index of the current item.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The source sequence of elements.</param>
        /// <param name="action">The action to invoke against each element in the sequence.</param>
        public static void ForEach<TType>(this IEnumerable<TType> items, Action<TType, int> action)
        {
            _ = items.WhenNotNull(nameof(items));

            var index = 0;

            foreach (var item in items)
            {
                action.Invoke(item, index++);
            }
        }

        /// <summary>Asynchronously iterates a sequence of elements and provides the zero-based index of the current item.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The source sequence of elements.</param>
        /// <param name="action">The asynchronous action to invoke against each element in the sequence.</param>
        /// <returns>An awaitable task that completes when the iteration is complete.</returns>
        public static async Task ForEachAsync<TType>(this IEnumerable<TType> items, Func<TType, int, Task> action)
        {
            _ = items.WhenNotNull(nameof(items));

            var index = 0;

            foreach (var item in items)
            {
                await action.Invoke(item, index++).ConfigureAwait(false);
            }
        }

        /// <summary>Finds elements in a collection that match elements in a second collection based on key selectors.</summary>
        /// <typeparam name="TFirst">The element type of the first collection.</typeparam>
        /// <typeparam name="TSecond">The element type of the second collection.</typeparam>
        /// <typeparam name="TKey">The key type used for comparing elements. Each element must have a unique key.</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="firstSelector">The key selector for each element in the first collection.</param>
        /// <param name="secondSelector">The key selector for each element in the second collection.</param>
        /// <returns>Elements from the first collection that have a key matching elements in the second collection.</returns>
        public static IEnumerable<TFirst> FindMatches<TFirst, TSecond, TKey>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second,
            Func<TFirst, TKey> firstSelector, Func<TSecond, TKey> secondSelector)
        {
            var firstItems = first
                .WhenNotNull(nameof(first))
                .AsReadOnlyCollection();

            var secondItems = second
                .WhenNotNull(nameof(second))
                .AsReadOnlyCollection();

            _ = firstSelector.WhenNotNull(nameof(firstSelector));
            _ = secondSelector.WhenNotNull(nameof(secondSelector));

            // This method is faster than using LINQ queries to find the matches
            var firstMap = firstItems.ToDictionary(firstSelector);
            var matchingKeys = firstMap.Keys.Intersect(secondItems.Select(secondSelector));
            return matchingKeys.Select(key => firstMap[key]);
        }

        /// <summary>Determines if a collection is unique when grouped by a specified key selector.</summary>
        /// <typeparam name="TType">The element type in the source collection.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The key selector.</param>
        /// <returns>True when the grouping results in one element per key, otherwise false.</returns>
        public static bool HasDistinctGrouping<TType, TKey>(this IEnumerable<TType> source, Func<TType, TKey> selector)
        {
            return source
                .WhenNotNull(nameof(source))
                .GroupBy(selector)
                .All(item => item.Count() == 1);
        }
    }
}