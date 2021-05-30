using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AllOverIt.Extensions
{
    public static class EnumerableExtensions
    {
        public static IList<TType> AsList<TType>(this IEnumerable<TType> items)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            return items is IList<TType> list
              ? list
              : items.ToList();
        }

        public static IReadOnlyList<TType> AsReadOnlyList<TType>(this IEnumerable<TType> items)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            return items is IReadOnlyList<TType> list
              ? list
              : items.ToList();
        }

        public static IReadOnlyCollection<TType> AsReadOnlyCollection<TType>(this IEnumerable<TType> items)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            return items is IReadOnlyCollection<TType> list
              ? list
              : new ReadOnlyCollection<TType>(items.AsList());
        }

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
            _ = items ?? throw new ArgumentNullException(nameof(items));

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
    }
}