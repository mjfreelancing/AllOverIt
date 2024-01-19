using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    // Iterating 'IAsyncEnumerable<TType> items' can be equally performed as shown below - the compiler will convert the
    // await foreach() to the use of an async enumerator.
    //
    //
    // await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
    // {
    //     cancellationToken.ThrowIfCancellationRequested();
    //
    //     ...do something with each item
    // }
    //
    //
    // await using (var enumerator = items.GetAsyncEnumerator(cancellationToken))
    // {
    //     while (await enumerator.MoveNextAsync())
    //     {
    //         cancellationToken.ThrowIfCancellationRequested();
    //
    //         ...do something with each item
    //     }
    // }
    //}

    /// <summary>Provides a variety of extension methods for <see cref="IAsyncEnumerable{T}"/>.</summary>
    public static class AsyncEnumerableExtensions
    {
        /// <summary>Asynchronously projects each item within a sequence.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An enumerator that provides asynchronous iteration over a sequence of elements.</returns>
        public static async IAsyncEnumerable<TResult> SelectAsync<TType, TResult>(this IAsyncEnumerable<TType> items, Func<TType, Task<TResult>> selector,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return await selector.Invoke(item).ConfigureAwait(false);
            }
        }

        /// <summary>Asynchronously projects each item within a sequence to an <see cref="IEnumerable{TResult}"/> and flattens the result to
        /// to a new sequence.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An enumerator that provides asynchronous iteration over a sequence of elements.</returns>
        public static async IAsyncEnumerable<TResult> SelectManyAsync<TType, TResult>(this IAsyncEnumerable<TType> items, Func<TType, IEnumerable<TResult>> selector,
           [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var results = selector.Invoke(item);

                foreach (var result in results)
                {
                    yield return result;
                }
            }
        }

        /// <summary>Iterates over an <see cref="IAsyncEnumerable{T}"/> to create a <see cref="List{T}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The enumerable to convert to a list asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>An <see cref="IList{T}"/> from the source items.</returns>
        public static async Task<IList<TType>> ToListAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var listItems = new List<TType>();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                listItems.Add(item);
            }

            return listItems;
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as an IList{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IList{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an IList{TResult}.</returns>
        public static async Task<IList<TResult>> SelectAsListAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items, Func<TSource, Task<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var listItems = new List<TResult>();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await selector.Invoke(item).ConfigureAwait(false);

                listItems.Add(result);
            }

            return listItems;
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as an IReadOnlyCollection{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyCollection{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an IReadOnlyCollection{TResult}.</returns>
        public static async Task<IReadOnlyCollection<TResult>> SelectAsReadOnlyCollectionAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            var results = await SelectAsListAsync(items, selector, cancellationToken).ConfigureAwait(false);

            return results.AsReadOnlyCollection();
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as an IReadOnlyList{TResult}.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyList{TResult}.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an IReadOnlyList{TResult}.</returns>
        public static async Task<IReadOnlyList<TResult>> SelectAsReadOnlyListAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            var results = await SelectAsListAsync(items, selector, cancellationToken).ConfigureAwait(false);

            return results.AsReadOnlyList();
        }

        /// <summary>Asynchronously iterates a sequence of elements and provides the zero-based index of the current item.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The source sequence of elements.</param>
        /// <param name="action">The action to invoke against each element in the sequence.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>An awaitable task that completes when the iteration is complete.</returns>
        public static async Task ForEachAsync<TType>(this IAsyncEnumerable<TType> items, Action<TType, int> action,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var index = 0;

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                action.Invoke(item, index++);
            }
        }

        /// <summary>Asynchronously iterates a sequence of elements and provides the zero-based index of the current item.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The source sequence of elements.</param>
        /// <param name="action">The asynchronous action to invoke against each element in the sequence.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>An awaitable task that completes when the iteration is complete.</returns>
        public static async Task ForEachAsync<TType>(this IAsyncEnumerable<TType> items, Func<TType, int, Task> action,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var index = 0;

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await action.Invoke(item, index++).ConfigureAwait(false);
            }
        }

        /// <summary>Asynchronously projects each element of a sequence into a new form that includes the element's zero-based index.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The source sequence of elements.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected sequence including the element's index.</returns>
        public static async IAsyncEnumerable<(TType Item, int Index)> WithIndexAsync<TType>(this IAsyncEnumerable<TType> items,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var index = 0;

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return (item, index++);
            }
        }
    }
}
