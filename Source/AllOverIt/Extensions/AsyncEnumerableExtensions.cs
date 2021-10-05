using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    // This works because we are only targeting NetStandard 2 and above
#if !NETSTANDARD2_0
    public static class AsyncEnumerableExtensions
    {
        // Not named 'ToListAsync' because this easily conflicts with other implementations of the same name (such as EF)
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