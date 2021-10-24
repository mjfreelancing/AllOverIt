using AllOverIt.Helpers;
using AllOverIt.Patterns.Specification;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IEnumerable{T}"/>.</summary>
    public static class EnumerableExtensions
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

        /// <summary>Gets all candidates that meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The candidates that meet the criteria of the provided specification.</returns>
        public static IEnumerable<TType> Where<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Where(specification.IsSatisfiedBy);
        }

        /// <summary>Determines if any candidates meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>True if any of the candidates meet the criteria of the provided specification.</returns>
        public static bool Any<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Any(specification.IsSatisfiedBy);
        }

        /// <summary>Determines if all candidates meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>True if all of the candidates meet the criteria of the provided specification.</returns>
        public static bool All<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.All(specification.IsSatisfiedBy);
        }

        /// <summary>Counts the number of candidates that meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The count of candidates that meet the criteria of a provided specification.</returns>
        public static int Count<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Count(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the first candidate that meets the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The first candidate that meets the criteria of a provided specification.</returns>
        public static TType First<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.First(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the first candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The first candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</returns>
        public static TType FirstOrDefault<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.FirstOrDefault(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the last candidate that meets the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The last candidate that meets the criteria of a provided specification.</returns>
        public static TType Last<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Last(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the last candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The last candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</returns>
        public static TType LastOrDefault<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.LastOrDefault(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the elements from the first element of the input candidates that meets the criteria of a provided
        /// specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>An enumerable that contains the elements from the first element of the input candidates that meets
        /// the criteria of a provided specification.</returns>
        public static IEnumerable<TType> SkipWhile<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.SkipWhile(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the elements from the input candidates while they meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>An enumerable that contains the elements from the input candidates while they meet the criteria of a provided
        /// specification.</returns>
        public static IEnumerable<TType> TakeWhile<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.TakeWhile(specification.IsSatisfiedBy);
        }

        // Parallel based processing derived from https://devblogs.microsoft.com/pfxteam/implementing-a-simple-foreachasync-part-2/

        #region ForEachAsTaskAsync

        /// <summary>Creates a Task for each item in a collection and invokes an asynchronous function. The number of tasks is
        /// partitioned based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of tasks that will be created.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsTaskAsync<TType>(this IEnumerable<TType> items, Func<TType, Task> func, int degreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .Select(partition =>
                    {
                        return Task.Run(async () =>
                        {
                            await ProcessPartitionAsync(partition, func, cancellationToken);
                        }, cancellationToken);
                    }));
        }

        /// <summary>Creates a Task for each item in a collection and invokes an asynchronous function. The number of tasks is
        /// partitioned based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput">The type of the additional input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input">An additional input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of tasks that will be created.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsTaskAsync<TType, TInput>(this IEnumerable<TType> items, Func<TType, TInput, Task> func, TInput input,
            int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .Select(partition =>
                    {
                        return Task.Run(async () =>
                        {
                            await ProcessPartitionAsync(partition, func, input, cancellationToken);
                        }, cancellationToken);
                    }));
        }

        /// <summary>Creates a Task for each item in a collection and invokes an asynchronous function. The number of tasks is
        /// partitioned based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput1">The type of the first input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput2">The type of the second input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input1">The first input that will be passed to the invoked function.</param>
        /// <param name="input2">The second input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of tasks that will be created.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsTaskAsync<TType, TInput1, TInput2>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, Task> func, 
            TInput1 input1, TInput2 input2, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .Select(partition =>
                    {
                        return Task.Run(async () =>
                        {
                            await ProcessPartitionAsync(partition, func, input1, input2, cancellationToken);
                        }, cancellationToken);
                    }));
        }

        /// <summary>Creates a Task for each item in a collection and invokes an asynchronous function. The number of tasks is
        /// partitioned based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput1">The type of the first input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput2">The type of the second input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput3">The type of the third input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input1">The first input that will be passed to the invoked function.</param>
        /// <param name="input2">The second input that will be passed to the invoked function.</param>
        /// <param name="input3">The third input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of tasks that will be created.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsTaskAsync<TType, TInput1, TInput2, TInput3>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, TInput3, Task> func,
            TInput1 input1, TInput2 input2, TInput3 input3, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .Select(partition =>
                    {
                        return Task.Run(async () =>
                        {
                            await ProcessPartitionAsync(partition, func, input1, input2, input3, cancellationToken);
                        }, cancellationToken);
                    }));
        }

        /// <summary>Creates a Task for each item in a collection and invokes an asynchronous function. The number of tasks is
        /// partitioned based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput1">The type of the first input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput2">The type of the second input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput3">The type of the third input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput4">The type of the forth input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input1">The first input that will be passed to the invoked function.</param>
        /// <param name="input2">The second input that will be passed to the invoked function.</param>
        /// <param name="input3">The third input that will be passed to the invoked function.</param>
        /// <param name="input4">The forth input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of tasks that will be created.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsTaskAsync<TType, TInput1, TInput2, TInput3, TInput4>(this IEnumerable<TType> items,
            Func<TType, TInput1, TInput2, TInput3, TInput4, Task> func, TInput1 input1, TInput2 input2, TInput3 input3, TInput4 input4,
            int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .Select(partition =>
                    {
                        return Task.Run(async () =>
                        {
                            await ProcessPartitionAsync(partition, func, input1, input2, input3, input4, cancellationToken);
                        }, cancellationToken);
                    }));
        }

        #endregion

        #region ForEachAsParallelAsync

        /// <summary>Parallelizes the invocation of a function against a collection of items. The parallelization is partitioned
        /// based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of parallel operations.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsParallelAsync<TType>(this IEnumerable<TType> items, Func<TType, Task> func, int degreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .AsParallel()
                    .Select(partition => ProcessPartitionAsync(partition, func, cancellationToken)));
        }

        /// <summary>Parallelizes the invocation of a function against a collection of items. The parallelization is partitioned
        /// based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput">The type of the additional input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input">An additional input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of parallel operations.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsParallelAsync<TType, TInput>(this IEnumerable<TType> items, Func<TType, TInput, Task> func, TInput input, int degreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .AsParallel()
                    .Select(partition => ProcessPartitionAsync(partition, func, input, cancellationToken)));
        }

        /// <summary>Parallelizes the invocation of a function against a collection of items. The parallelization is partitioned
        /// based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput1">The type of the first input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput2">The type of the second input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input1">The first input that will be passed to the invoked function.</param>
        /// <param name="input2">The second input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of parallel operations.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsParallelAsync<TType, TInput1, TInput2>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, Task> func,
            TInput1 input1, TInput2 input2, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .AsParallel()
                    .Select(partition => ProcessPartitionAsync(partition, func, input1, input2, cancellationToken)));
        }

        /// <summary>Parallelizes the invocation of a function against a collection of items. The parallelization is partitioned
        /// based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput1">The type of the first input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput2">The type of the second input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput3">The type of the third input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input1">The first input that will be passed to the invoked function.</param>
        /// <param name="input2">The second input that will be passed to the invoked function.</param>
        /// <param name="input3">The third input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of parallel operations.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsParallelAsync<TType, TInput1, TInput2, TInput3>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, TInput3, Task> func,
            TInput1 input1, TInput2 input2, TInput3 input3, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .AsParallel()
                    .Select(partition => ProcessPartitionAsync(partition, func, input1, input2, input3, cancellationToken)));
        }

        /// <summary>Parallelizes the invocation of a function against a collection of items. The parallelization is partitioned
        /// based on a provided degree of parallelism.</summary>
        /// <typeparam name="TType">The type of each element.</typeparam>
        /// <typeparam name="TInput1">The type of the first input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput2">The type of the second input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput3">The type of the third input that will be passed to the invoked function.</typeparam>
        /// <typeparam name="TInput4">The type of the forth input that will be passed to the invoked function.</typeparam>
        /// <param name="items">The collection of items to be processed.</param>
        /// <param name="func">The asynchronous function to be invoked.</param>
        /// <param name="input1">The first input that will be passed to the invoked function.</param>
        /// <param name="input2">The second input that will be passed to the invoked function.</param>
        /// <param name="input3">The third input that will be passed to the invoked function.</param>
        /// <param name="input4">The forth input that will be passed to the invoked function.</param>
        /// <param name="degreeOfParallelism">Determines the maximum number of parallel operations.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the processing.</param>
        /// <returns>A task that will complete when all items have been processed.</returns>
        public static Task ForEachAsParallelAsync<TType, TInput1, TInput2, TInput3, TInput4>(this IEnumerable<TType> items,
            Func<TType, TInput1, TInput2, TInput3, TInput4, Task> func, TInput1 input1, TInput2 input2, TInput3 input3, TInput4 input4,
            int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(
                items
                    .GetPartitions(degreeOfParallelism)
                    .AsParallel()
                    .Select(partition => ProcessPartitionAsync(partition, func, input1, input2, input3, input4, cancellationToken)));
        }

        #endregion

        private static IList<IEnumerator<TType>> GetPartitions<TType>(this IEnumerable<TType> items, int partitionCount)
        {
            return Partitioner.Create(items).GetPartitions(partitionCount);
        }

        #region ProcessPartitionAsync

        private static async Task ProcessPartitionAsync<TType>(IEnumerator<TType> partition, Func<TType, Task> func, CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput>(IEnumerator<TType> partition, Func<TType, TInput, Task> func, TInput input,
            CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput1, TInput2>(IEnumerator<TType> partition, Func<TType, TInput1, TInput2, Task> func,
            TInput1 input1, TInput2 input2, CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input1, input2).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput1, TInput2, TInput3>(IEnumerator<TType> partition, Func<TType, TInput1, TInput2, TInput3, Task> func,
            TInput1 input1, TInput2 input2, TInput3 input3, CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input1, input2, input3).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput1, TInput2, TInput3, TInput4>(IEnumerator<TType> partition,
            Func<TType, TInput1, TInput2, TInput3, TInput4, Task> func, TInput1 input1, TInput2 input2, TInput3 input3, TInput4 input4,
            CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input1, input2, input3, input4).ConfigureAwait(false);
                }
            }
        }

        #endregion
    }
}