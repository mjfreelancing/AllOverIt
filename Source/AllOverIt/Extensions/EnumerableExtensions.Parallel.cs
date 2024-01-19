using AllOverIt.Assertion;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    public static partial class EnumerableExtensions
    {
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
        public static Task ForEachAsTaskAsync<TType>(this IEnumerable<TType> items, Func<TType, CancellationToken, Task> func, int degreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
        public static Task ForEachAsTaskAsync<TType, TInput>(this IEnumerable<TType> items, Func<TType, TInput, CancellationToken, Task> func, TInput input,
            int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
        public static Task ForEachAsTaskAsync<TType, TInput1, TInput2>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, CancellationToken, Task> func, 
            TInput1 input1, TInput2 input2, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
        public static Task ForEachAsTaskAsync<TType, TInput1, TInput2, TInput3>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, TInput3, CancellationToken, Task> func,
            TInput1 input1, TInput2 input2, TInput3 input3, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
            Func<TType, TInput1, TInput2, TInput3, TInput4, CancellationToken, Task> func, TInput1 input1, TInput2 input2, TInput3 input3, TInput4 input4,
            int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
        public static Task ForEachAsParallelAsync<TType>(this IEnumerable<TType> items, Func<TType, CancellationToken, Task> func, int degreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
        public static Task ForEachAsParallelAsync<TType, TInput>(this IEnumerable<TType> items, Func<TType, TInput, CancellationToken, Task> func, TInput input, int degreeOfParallelism,
            CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
        public static Task ForEachAsParallelAsync<TType, TInput1, TInput2>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, CancellationToken, Task> func,
            TInput1 input1, TInput2 input2, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
        public static Task ForEachAsParallelAsync<TType, TInput1, TInput2, TInput3>(this IEnumerable<TType> items, Func<TType, TInput1, TInput2, TInput3, CancellationToken, Task> func,
            TInput1 input1, TInput2 input2, TInput3 input3, int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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
            Func<TType, TInput1, TInput2, TInput3, TInput4, CancellationToken, Task> func, TInput1 input1, TInput2 input2, TInput3 input3, TInput4 input4,
            int degreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _ = items.WhenNotNull(nameof(items));
            _ = func.WhenNotNull(nameof(func));

            Throw<ArgumentOutOfRangeException>.When(degreeOfParallelism < 1, nameof(degreeOfParallelism), "At least one task must be specified.");

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

        private static async Task ProcessPartitionAsync<TType>(IEnumerator<TType> partition, Func<TType, CancellationToken, Task> func, CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput>(IEnumerator<TType> partition, Func<TType, TInput, CancellationToken, Task> func, TInput input,
            CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput1, TInput2>(IEnumerator<TType> partition, Func<TType, TInput1, TInput2, CancellationToken, Task> func,
            TInput1 input1, TInput2 input2, CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input1, input2, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput1, TInput2, TInput3>(IEnumerator<TType> partition, Func<TType, TInput1, TInput2, TInput3, CancellationToken, Task> func,
            TInput1 input1, TInput2 input2, TInput3 input3, CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input1, input2, input3, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static async Task ProcessPartitionAsync<TType, TInput1, TInput2, TInput3, TInput4>(IEnumerator<TType> partition,
            Func<TType, TInput1, TInput2, TInput3, TInput4, CancellationToken, Task> func, TInput1 input1, TInput2 input2, TInput3 input3, TInput4 input4,
            CancellationToken cancellationToken)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await func.Invoke(partition.Current, input1, input2, input3, input4, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        #endregion
    }
}