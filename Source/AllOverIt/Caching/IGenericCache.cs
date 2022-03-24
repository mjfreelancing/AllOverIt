using System;
using System.Collections.Generic;

namespace AllOverIt.Caching
{
    /// <summary>Represents a generic cache for storing any object type based on a custom key.</summary>
    public interface IGenericCache : IEnumerable<KeyValuePair<GenericCacheKeyBase, object>>
    {
#if NET6_0_OR_GREATER
        IEqualityComparer<GenericCacheKeyBase> Comparer { get; }
#endif

        int Count { get; }

        bool IsEmpty { get; }

        ICollection<GenericCacheKeyBase> Keys { get; }

        ICollection<object> Values { get; }

        object this[GenericCacheKeyBase key] { get; }

        bool TryAdd<TValue>(GenericCacheKeyBase key, TValue value);

        bool ContainsKey(GenericCacheKeyBase key);

        bool TryRemove<TValue>(GenericCacheKeyBase key, out TValue value);

#if NET5_0_OR_GREATER
        bool TryRemove<TValue>(KeyValuePair<GenericCacheKeyBase, TValue> item);
#endif

        bool TryGetValue<TValue>(GenericCacheKeyBase key, out TValue value);

        bool TryUpdate<TValue>(GenericCacheKeyBase key, TValue newValue, TValue comparisonValue);

        void Clear();

        KeyValuePair<GenericCacheKeyBase, object>[] ToArray();

        /// <summary>Gets the existing value of a key if it exists, otherwise adds a new value based on a provided resolver.</summary>
        /// <typeparam name="TValue">The value type associated with the key.</typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="resolver">The value factory for the provided key when the key does not exist.</param>
        /// <returns>The existing value of a key if it exists, otherwise a newly added value.</returns>
        TValue GetOrAdd<TValue>(GenericCacheKeyBase key, Func<GenericCacheKeyBase, TValue> resolver);

#if !NETSTANDARD2_0
        TValue GetOrAdd<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg, TValue> resolver,
            TArg resolverArgument);
#endif

        TValue GetOrAdd<TValue>(GenericCacheKeyBase key, TValue value);

#if !NETSTANDARD2_0
        TValue AddOrUpdate<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg, TValue> addResolver,
            Func<GenericCacheKeyBase, TValue, TArg, TValue> updateResolver,
            TArg resolverArgument);
#endif

        TValue AddOrUpdate<TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TValue> addResolver,
            Func<GenericCacheKeyBase, TValue, TValue> updateResolver);

        TValue AddOrUpdate<TValue>(
            GenericCacheKeyBase key,
            TValue addValue,
            Func<GenericCacheKeyBase, TValue, TValue> updateResolver);
    }
}