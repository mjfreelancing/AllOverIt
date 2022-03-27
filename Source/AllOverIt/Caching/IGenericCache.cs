using System;
using System.Collections.Generic;

namespace AllOverIt.Caching
{
    /// <summary>Represents a generic cache for storing any object type based on a custom key.</summary>
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IGenericCache : IDictionary<GenericCacheKeyBase, object>, IReadOnlyDictionary<GenericCacheKeyBase, object>
    {
        // Properties and methods defined on both ICollection<KeyValuePair<GenericCacheKeyBase, object>> and IReadOnlyCollection<KeyValuePair<GenericCacheKeyBase, object>>  or
        // IDictionary<GenericCacheKeyBase, object> and IReadOnlyDictionary<GenericCacheKeyBase, object>
        //
        // int Count { get; }
        // ICollection<GenericCacheKeyBase> Keys { get; }
        // ICollection<object> Values { get; }
        // bool ContainsKey(GenericCacheKeyBase key);
        // bool TryGetValue(GenericCacheKeyBase key, out object value);
        // object this[GenericCacheKeyBase key] { get; }

        void Add<TValue>(GenericCacheKeyBase key, TValue value);

        bool TryAdd<TValue>(GenericCacheKeyBase key, TValue value);

        bool TryGetValue<TValue>(GenericCacheKeyBase key, out TValue value);

        bool TryRemove<TValue>(GenericCacheKeyBase key, out TValue value);

#if NET5_0_OR_GREATER
        bool TryRemove<TValue>(KeyValuePair<GenericCacheKeyBase, TValue> item);
#endif

        bool TryUpdate<TValue>(GenericCacheKeyBase key, TValue newValue, TValue comparisonValue);

        KeyValuePair<GenericCacheKeyBase, object>[] ToArray();

        /// <summary>Gets the existing value of a key if it exists, otherwise adds a new value based on a provided resolver.</summary>
        /// <typeparam name="TValue">The value type associated with the key.</typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="addResolver">The value factory for the provided key when the key does not exist.</param>
        /// <returns>The existing value of a key if it exists, otherwise a newly added value.</returns>
        TValue GetOrAdd<TValue>(GenericCacheKeyBase key, Func<GenericCacheKeyBase, TValue> addResolver);

        TValue GetOrAdd<TValue>(GenericCacheKeyBase key, TValue value);

#if !NETSTANDARD2_0
        TValue GetOrAdd<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg, TValue> addResolver,
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

#if !NETSTANDARD2_0
        TValue AddOrUpdate<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg, TValue> addResolver,
            Func<GenericCacheKeyBase, TValue, TArg, TValue> updateResolver,
            TArg resolverArgument);
#endif
    }
}