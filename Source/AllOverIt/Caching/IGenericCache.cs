namespace AllOverIt.Caching
{
    /// <summary>A generic cache capable of storing different key/value types. Each key type must inherit <see cref="GenericCacheKeyBase"/>
    /// and each of the key elements must support equality comparison.</summary>
    public interface IGenericCache : IDictionary<GenericCacheKeyBase, object?>, IReadOnlyDictionary<GenericCacheKeyBase, object?>
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

        /// <summary>Adds the provided key and value to the cache.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The custom key associated with the value.</param>
        /// <param name="value">The value associated with the key.</param>
        void Add<TValue>(GenericCacheKeyBase key, TValue? value);

        /// <summary>Attempts to add the specified key and associated value into the cache.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The custom key associated with the value.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns><see langword="true" /> if the key and associated value were added, otherwise <see langword="false" />.</returns>
        bool TryAdd<TValue>(GenericCacheKeyBase key, TValue? value);

        /// <summary>Attempts to get the value associated with a key in the cache.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The custom key associated with the value.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns><see langword="true" /> if the key was found in the cache, otherwise <see langword="false" />.</returns>
        bool TryGetValue<TValue>(GenericCacheKeyBase key, out TValue? value);

        /// <summary>Attempts to remove a key from the cache and return the value associated with the key if it was found.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The custom key associated with the value.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns><see langword="true" /> if the key was found in the cache, otherwise <see langword="false" />.</returns>
        bool TryRemove<TValue>(GenericCacheKeyBase key, out TValue? value);

#if !NETSTANDARD2_1
        /// <summary>Attempts to remove a key and its associated value from the cache.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="item">The custom key and associated value.</param>
        /// <returns><see langword="true" /> if the key was found in the cache, otherwise <see langword="false" />.</returns>
        bool TryRemove<TValue>(KeyValuePair<GenericCacheKeyBase, TValue?> item);
#endif

        /// <summary>Updates the value associated with the specified key to <paramref name="newValue"/> if the existing value is
        /// equal to <paramref name="comparisonValue"/>.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The custom key associated with the value.</param>
        /// <param name="newValue">The new value to update to.</param>
        /// <param name="comparisonValue">The value to compare with the current value.</param>
        /// <returns><see langword="true" /> if the key was found in the cache, otherwise <see langword="false" />.</returns>
        bool TryUpdate<TValue>(GenericCacheKeyBase key, TValue? newValue, TValue? comparisonValue);

        /// <summary>Copies the key and value pairs to a new array.</summary>
        /// <returns>A new array of the cache's current key and value pairs.</returns>
        KeyValuePair<GenericCacheKeyBase, object>[] ToArray();

        /// <summary>Gets the existing value of a key if it exists, otherwise adds a new value based on a provided resolver.</summary>
        /// <typeparam name="TValue">The value type associated with the key.</typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="addResolver">The value factory for the provided key when the key does not exist.</param>
        /// <returns>The existing value of a key if it exists, otherwise the newly added value.</returns>
        TValue? GetOrAdd<TValue>(GenericCacheKeyBase key, Func<GenericCacheKeyBase, TValue?> addResolver);

        /// <summary>Gets the existing value of a key if it exists, otherwise adds a new value.</summary>
        /// <typeparam name="TValue">The value type associated with the key.</typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="value">The value to add if the cache does not contain the key.</param>
        /// <returns>The existing value of a key if it exists, otherwise the newly added value.</returns>
        TValue? GetOrAdd<TValue>(GenericCacheKeyBase key, TValue? value);

        /// <summary>Adds a key/value pair to the cache using a specified resolver and an argument when the key does not exist,
        /// otherwise returns the existing value.</summary>
        /// <typeparam name="TArg">The argument type.</typeparam>
        /// <typeparam name="TValue">The value type associated with the key.</typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="addResolver">The value factory for the provided key when the key does not exist.</param>
        /// <param name="resolverArgument">The argument factory for the provided key when the key does not exist.</param>
        /// <returns>The existing value of a key if it exists, otherwise the newly added value.</returns>
        TValue? GetOrAdd<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg?, TValue?> addResolver,
            TArg resolverArgument);

        /// <summary>Adds a key/value pair to the cache using a specified resolver and an argument when the key does not exist,
        /// otherwise updates the existing value.</summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="addResolver">The add value factory for the provided key when the key does not exist.</param>
        /// <param name="updateResolver">The update value factory for the provided key when the key already exist.</param>
        /// <returns>The existing value of a key if it exists, otherwise the newly updated value.</returns>
        TValue? AddOrUpdate<TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TValue?> addResolver,
            Func<GenericCacheKeyBase, TValue?, TValue?> updateResolver);

        /// <summary>Adds a key/value pair to the cache when the key does not exist, otherwise updates the existing value using a
        /// specified value factory.</summary>
        /// <typeparam name="TValue">The value type associated with the key.</typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="addValue">The value to add when the key does not exist.</param>
        /// <param name="updateResolver">The update value factory for the provided key when the key already exist.</param>
        /// <returns>The existing value of a key if it exists, otherwise the newly updated value.</returns>
        TValue? AddOrUpdate<TValue>(
            GenericCacheKeyBase key,
            TValue? addValue,
            Func<GenericCacheKeyBase, TValue?, TValue?> updateResolver);

        /// <summary>Adds a key/value pair to the cache using a specified resolver and an argument when the key does not exist,
        /// updates the existing value using a specified value factory and an argument.</summary>
        /// <typeparam name="TArg">The argument type.</typeparam>
        /// <typeparam name="TValue">The value type associated with the key.</typeparam>
        /// <param name="key">The custom key.</param>
        /// <param name="addResolver">The value factory for the provided key when the key does not exist.</param>
        /// <param name="updateResolver">The update value factory for the provided key when the key already exist.</param>
        /// <param name="resolverArgument">The argument factory for the provided key when the key does not exist.</param>
        /// <returns>The existing value of a key if it exists, otherwise the newly updated value.</returns>
        TValue? AddOrUpdate<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg?, TValue?> addResolver,
            Func<GenericCacheKeyBase, TValue?, TArg?, TValue?> updateResolver,
            TArg? resolverArgument);
    }
}