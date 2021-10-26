using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    // This works because we are only targeting NetStandard 2 and above
#if !NETSTANDARD2_0
    /// <summary>Provides a variety of extension methods for <see cref="IAsyncEnumerable{T}"/>.</summary>
    public static class AsyncEnumerableExtensions
    {
        // Not named 'ToListAsync' because this easily conflicts with other implementations of the same name (such as EF)
        /// <summary>Iterates over an <see cref="IAsyncEnumerable{T}"/> to create a <see cref="List{T}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The enumerable to convert to a list asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>An <see cref="IList{T}"/> from the source items.</returns>
        public static async Task<IList<TType>> AsListAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
        {
            var listItems = new List<TType>();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                listItems.Add(item);
            }

            return listItems;
        }

        public static async Task<IList<TResult>> SelectAsListAsync<TType, TResult>(this IAsyncEnumerable<TType> items, Func<TType, Task<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            _ = items.WhenNotNull(nameof(items));

            var listItems = new List<TResult>();

            // ReSharper disable once PossibleMultipleEnumeration
            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await selector.Invoke(item).ConfigureAwait(false);
                listItems.Add(result);
            }

            return listItems;
        }

        public static async Task<IReadOnlyCollection<TResult>> SelectAsReadOnlyCollectionAsync<TType, TResult>(this IAsyncEnumerable<TType> items, Func<TType, Task<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            var results = await SelectAsListAsync(items, selector, cancellationToken);
            return results.AsReadOnlyCollection();
        }

        public static async Task<IReadOnlyList<TResult>> SelectAsReadOnlyListAsync<TType, TResult>(this IAsyncEnumerable<TType> items, Func<TType, Task<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            var results = await SelectAsListAsync(items, selector, cancellationToken);
            return results.AsReadOnlyList();
        }
    }
#endif
}