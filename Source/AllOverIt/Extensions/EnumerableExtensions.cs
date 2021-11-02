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
        /// <summary>Returns the source items as an IList.</summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be returned as an IList.</param>
        /// <returns>If the source items is already an IList. If the source is already an IList then the same reference is returned,
        /// otherwise a new list is created and populated before being returned.</returns>
        public static IList<TType> AsList<TType>(this IEnumerable<TType> items)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            // ReSharper disable once PossibleMultipleEnumeration
            return items is IList<TType> list
              ? list
              : items.ToList();
        }

        /// <summary>Returns the source items as an IReadOnlyList.</summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be returned as an IReadOnlyList.</param>
        /// <returns>The source items as an IReadOnlyList. If the source is already an IReadOnlyList then the same
        /// reference is returned, otherwise a new list is created and populated before being returned.</returns>
        public static IReadOnlyList<TType> AsReadOnlyList<TType>(this IEnumerable<TType> items)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            // ReSharper disable once PossibleMultipleEnumeration
            return items is IReadOnlyList<TType> list
              ? list
              : items.ToList();
        }

        /// <summary>Returns the source items as an IReadOnlyCollection.</summary>
        /// <typeparam name="TType">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be returned as an IReadOnlyCollection.</param>
        /// <returns>The source items as an IReadOnlyCollection. If the source is already an IReadOnlyCollection then the same
        /// reference is returned, otherwise a new list is created and populated before being returned.</returns>
        public static IReadOnlyCollection<TType> AsReadOnlyCollection<TType>(this IEnumerable<TType> items)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            // ReSharper disable once PossibleMultipleEnumeration
            return items is IReadOnlyCollection<TType> list
              ? list
              : new ReadOnlyCollection<TType>(items.AsList());
        }

        /// <summary>Projects each element into another form and returns the result as an IList{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IList{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <returns>The projected results as an IList{TResult}.</returns>
        public static IList<TResult> SelectAsList<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selector)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            // ReSharper disable once PossibleMultipleEnumeration
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
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            // ReSharper disable once PossibleMultipleEnumeration
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
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            // ReSharper disable once PossibleMultipleEnumeration
            return items.Select(selector).ToList();
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
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return await selector.Invoke(item).ConfigureAwait(false);
            }
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
        /// <typeparam name="TSource">The type stored in the source collection.</typeparam>
        /// <param name="items">The source of items to be partitioned.</param>
        /// <param name="batchSize">The maximum number of items in each batch.</param>
        /// <returns>One or more batches containing the source items partitioned into a maximum batch size.</returns>
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> items, int batchSize)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            var batch = new List<TSource>(batchSize);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                batch.Add(item);

                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<TSource>(batchSize);
                }
            }

            if (batch.Any())
            {
                yield return batch;
            }
        }
    }
}