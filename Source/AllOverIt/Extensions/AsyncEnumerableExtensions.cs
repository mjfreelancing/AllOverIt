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
        /// <summary>Iterates over an <see cref="IAsyncEnumerable{T}"/> of type <typeparamref name="TType"/> to create a <see cref="ReadOnlyCollection{T}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The enumerable to convert to a list asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> from the source items.</returns>
        public static ValueTask<ReadOnlyCollection<TType>> ToReadOnlyCollectionAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();

            return Impl(items.WithCancellation(cancellationToken).ConfigureAwait(false));

            static async ValueTask<ReadOnlyCollection<TType>> Impl(ConfiguredCancelableAsyncEnumerable<TType> source)
            {
                List<TType> list = [];

                await foreach (TType element in source)
                {
                    list.Add(element);
                }

                return list.AsReadOnly();
            }
        }

        /// <summary>Asynchronously projects each item within a sequence.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An enumerator that provides asynchronous iteration over a sequence of elements.</returns>
        public static IAsyncEnumerable<TResult> SelectAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, CancellationToken, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();
            _ = selector.WhenNotNull();

            return Impl(items.WithCancellation(cancellationToken).ConfigureAwait(false), selector, cancellationToken);

            static async IAsyncEnumerable<TResult> Impl(ConfiguredCancelableAsyncEnumerable<TType> source,
                Func<TType, CancellationToken, Task<TResult>> selector, [EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await foreach (var item in source)
                {
                    yield return await selector.Invoke(item, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to a new sequence.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An enumerator that provides asynchronous iteration over a sequence of elements.</returns>
        public static async IAsyncEnumerable<TResult> SelectManyAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, IEnumerable<TResult>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();
            _ = selector.WhenNotNull();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var results = selector.Invoke(item);

                foreach (var result in results)
                {
                    yield return result;
                }
            }
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to an array.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An array containing a flattened list of projected items.</returns>
        public static ValueTask<TResult[]> SelectManyToArrayAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, IEnumerable<TResult>> selector, CancellationToken cancellationToken = default)
        {
            // SelectManyAsync() asserts items and selector
            _ = selector.WhenNotNull();

            return items
                .SelectManyAsync(selector, cancellationToken)
                .ToArrayAsync(cancellationToken);
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to a <see cref="List{T}"/>.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="List{T}"/> containing a flattened list of projected items.</returns>
        public static ValueTask<List<TResult>> SelectManyToListAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, IEnumerable<TResult>> selector, CancellationToken cancellationToken = default)
        {
            // SelectManyAsync() asserts items and selector
            return items
                .SelectManyAsync(selector, cancellationToken)
                .ToListAsync(cancellationToken);
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to a <see cref="ReadOnlyCollection{T}"/>.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing a flattened list of projected items.</returns>
        public static ValueTask<ReadOnlyCollection<TResult>> SelectManyToReadOnlyCollectionAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
           Func<TType, IEnumerable<TResult>> selector, CancellationToken cancellationToken = default)
        {
            // SelectManyAsync() asserts items and selector
            return items
                .SelectManyAsync(selector, cancellationToken)
                .ToReadOnlyCollectionAsync(cancellationToken);
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to a new asynchronous sequence.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An enumerator that provides asynchronous iteration over a sequence of elements.</returns>
        public static async IAsyncEnumerable<TResult> SelectManyAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, CancellationToken, Task<IEnumerable<TResult>>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();
            _ = selector.WhenNotNull();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var results = await selector.Invoke(item, cancellationToken).ConfigureAwait(false);

                foreach (var result in results)
                {
                    yield return result;
                }
            }
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to an array.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An array containing a flattened list of projected items.</returns>
        public static ValueTask<TResult[]> SelectManyToArrayAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, CancellationToken, Task<IEnumerable<TResult>>> selector, CancellationToken cancellationToken = default)
        {
            // SelectManyAsync() asserts items and selector
            return items
                .SelectManyAsync(selector, cancellationToken)
                .ToArrayAsync(cancellationToken);
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to a <see cref="List{T}"/>.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="List{T}"/> containing a flattened list of projected items.</returns>
        public static ValueTask<List<TResult>> SelectManyToListAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, CancellationToken, Task<IEnumerable<TResult>>> selector, CancellationToken cancellationToken = default)
        {
            // SelectManyAsync() asserts items and selector
            return items
                .SelectManyAsync(selector, cancellationToken)
                .ToListAsync(cancellationToken);
        }

        /// <summary>Asynchronously projects and flattens a provided sequence to a <see cref="ReadOnlyCollection{T}"/>.</summary>
        /// <typeparam name="TType">The type of each element to be projected.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The sequence of elements to be projected.</param>
        /// <param name="selector">The transform function to be applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing a flattened list of projected items.</returns>
        public static ValueTask<ReadOnlyCollection<TResult>> SelectManyToReadOnlyCollectionAsync<TType, TResult>(this IAsyncEnumerable<TType> items,
            Func<TType, CancellationToken, Task<IEnumerable<TResult>>> selector, CancellationToken cancellationToken = default)
        {
            // SelectManyAsync() asserts items and selector
            return items
                .SelectManyAsync(selector, cancellationToken)
                .ToReadOnlyCollectionAsync(cancellationToken);
        }

#if !NET10_0_OR_GREATER
        /// <summary>Iterates over an <see cref="IAsyncEnumerable{T}"/> of type <typeparamref name="TType"/> to create a <typeparamref name="TType"/>[].</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The enumerable to convert to an array asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>A <typeparamref name="TType"/>[] from the source items.</returns>
        public static ValueTask<TType[]> ToArrayAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();

            return Impl(items.WithCancellation(cancellationToken).ConfigureAwait(false));

            static async ValueTask<TType[]> Impl(ConfiguredCancelableAsyncEnumerable<TType> source)
            {
                List<TType> list = [];

                await foreach (TType element in source)
                {
                    list.Add(element);
                }

                return [.. list];
            }
        }

        /// <summary>Iterates over an <see cref="IAsyncEnumerable{T}"/> of type <typeparamref name="TType"/> to create a <see cref="List{T}"/>.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The enumerable to convert to a list asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>A <see cref="List{T}"/> from the source items.</returns>
        public static ValueTask<List<TType>> ToListAsync<TType>(this IAsyncEnumerable<TType> items, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();

            return Impl(items.WithCancellation(cancellationToken).ConfigureAwait(false));

            static async ValueTask<List<TType>> Impl(ConfiguredCancelableAsyncEnumerable<TType> source)
            {
                List<TType> list = [];

                await foreach (TType element in source)
                {
                    list.Add(element);
                }

                return list;
            }
        }
#endif

        /// <summary>Asynchronously projects each element into another form and returns the result as a <typeparamref name="TResult"/>[].</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as a <typeparamref name="TResult"/>[].</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as a <typeparamref name="TResult"/>[].</returns>
        public static async ValueTask<TResult[]> SelectToArrayAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, TResult> selector, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();

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
        public static ValueTask<List<TResult>> SelectToListAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, TResult> selector, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();

            return Impl(items.WithCancellation(cancellationToken).ConfigureAwait(false), selector);

            static async ValueTask<List<TResult>> Impl(ConfiguredCancelableAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
            {
                List<TResult> list = [];

                await foreach (TSource element in source)
                {
                    var result = selector.Invoke(element);

                    list.Add(result);
                }

                return list;
            }
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as an <c>ReadOnlyCollection&lt;TResult&gt;</c>.</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as an <c>ReadOnlyCollection&lt;TResult&gt;</c>.</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as an <c>ReadOnlyCollection&lt;TResult&gt;</c>.</returns>
        public static async ValueTask<ReadOnlyCollection<TResult>> SelectToReadOnlyCollectionAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, TResult> selector, CancellationToken cancellationToken = default)
        {
            var list = await SelectToListAsync(items, selector, cancellationToken).ConfigureAwait(false);

            return list.AsReadOnly();
        }

        /// <summary>Asynchronously projects each element into another form and returns the result as a <typeparamref name="TResult"/>[].</summary>
        /// <typeparam name="TSource">The source elements.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="items">The source items to be projected and returned as a <typeparamref name="TResult"/>[].</param>
        /// <param name="selector">The transform function applied to each element.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the processing.</param>
        /// <returns>The projected results as a <typeparamref name="TResult"/>[].</returns>
        public static async ValueTask<TResult[]> SelectToArrayAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, CancellationToken, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();

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
        public static async ValueTask<List<TResult>> SelectToListAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, CancellationToken, Task<TResult>> selector, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull();

            var listItems = new List<TResult>();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var result = await selector.Invoke(item, cancellationToken).ConfigureAwait(false);

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
        public static async ValueTask<ReadOnlyCollection<TResult>> SelectToReadOnlyCollectionAsync<TSource, TResult>(this IAsyncEnumerable<TSource> items,
            Func<TSource, CancellationToken, Task<TResult>> selector, CancellationToken cancellationToken = default)
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
            _ = items.WhenNotNull();

            var index = 0;

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
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
            _ = items.WhenNotNull();

            var index = 0;

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
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
            _ = items.WhenNotNull();

            var index = 0;

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return (item, index++);
            }
        }
    }
}
