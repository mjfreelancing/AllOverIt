using AllOverIt.Assertion;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

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

        /// <summary>Asynchronously projects each item within a sequence to an <see cref="IEnumerable&lt;TResult&gt;"/> and flattens the result to
        /// to a new sequence.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An enumerator that provides asynchronous iteration over a sequence of elements.</returns>
        public static async IAsyncEnumerable<TResult> SelectManyAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, CancellationToken, IEnumerable<TResult>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var results = selector.Invoke(item, cancellationToken);

                foreach (var result in results)
                {
                    yield return result;
                }
            }
        }

        /// <summary>Iterates over an <see cref="IAsyncEnumerable{T}"/> of type <typeparamref name="TType"/> to create a <typeparamref name="TType"/>[].</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The enumerable to convert to an array asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>A <typeparamref name="TType"/>[] from the source items.</returns>
        public static async Task<TType[]> ToArrayAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var listItems = await items
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return [.. listItems];
        }

        /// <summary>Iterates over an <see cref="IAsyncEnumerable{T}"/> of type <typeparamref name="TType"/> to create a <see cref="List{T}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The enumerable to convert to a list asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>A <see cref="List{T}"/> from the source items.</returns>
        public static async Task<List<TType>> ToListAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
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

        #region Obsolete

        /// <summary>Asynchronously projects each element into another form and returns the result as an IList&lt;TResult&gt;.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IList&lt;TResult&gt;.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an IList&lt;TResult&gt;.</returns>
        [Obsolete("This method will be dropped in v8. Use SelectToListAsync() instead.")]
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

        /// <summary>Asynchronously projects each element into another form and returns the result as an IReadOnlyCollection&lt;TResult&gt;.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyCollection&lt;TResult&gt;.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an IReadOnlyCollection&lt;TResult&gt;.</returns>
        [Obsolete("This method will be dropped in v8. Use SelectToReadOnlyCollectionAsync() instead.")]
        public static async Task<IReadOnlyCollection<TResult>> SelectAsReadOnlyCollectionAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            var results = await SelectAsListAsync(items, selector, cancellationToken).ConfigureAwait(false);

            return results.AsReadOnlyCollection();
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as an IReadOnlyList&lt;TResult&gt;.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an IReadOnlyList&lt;TResult&gt;.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an IReadOnlyList&lt;TResult&gt;.</returns>
        [Obsolete("This method will be dropped in v8. Use SelectToReadOnlyListAsync() instead.")]
        public static async Task<IReadOnlyList<TResult>> SelectAsReadOnlyListAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            var results = await SelectAsListAsync(items, selector, cancellationToken).ConfigureAwait(false);

            return results.AsReadOnlyList();
        }

        #endregion

        /// <summary>Asynchronously projects each element into another form and returns the result as a <typeparamref name="TResult"/>[].</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as a <typeparamref name="TResult"/>[].</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as a <typeparamref name="TResult"/>[].</returns>
        public static async Task<TResult[]> SelectToArrayAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items, Func<TSource, Task<TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));

            var listItems = await SelectToListAsync(items, selector, cancellationToken).ConfigureAwait(false);

            return [.. listItems];
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as a <c>List&lt;TResult&gt;</c>.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as a <c>List&lt;TResult&gt;</c>.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as a <c>List&lt;TResult&gt;</c>.</returns>
        public static async Task<List<TResult>> SelectToListAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items, Func<TSource, Task<TResult>> selector,
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

        /// <summary>Asynchronously projects each element into another form and returns the result as an <c>ReadOnlyCollection&lt;TResult&gt;</c>.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an <c>ReadOnlyCollection&lt;TResult&gt;</c>.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an <c>ReadOnlyCollection&lt;TResult&gt;</c>.</returns>
        public static async Task<ReadOnlyCollection<TResult>> SelectToReadOnlyCollectionAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            var list = await SelectToListAsync(items, selector, cancellationToken).ConfigureAwait(false);

            return list.AsReadOnly();
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
