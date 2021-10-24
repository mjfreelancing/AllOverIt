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
        /// <returns>A <see cref="List{T}"/> from the source items.</returns>
        public static async Task<List<TType>> AsListAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
        {
            var listItems = new List<TType>();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                listItems.Add(item);
            }

            return listItems;
        }
    }
#endif
}