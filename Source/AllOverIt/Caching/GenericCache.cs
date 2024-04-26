using AllOverIt.Assertion;
using System.Collections;
using System.Collections.Concurrent;

namespace AllOverIt.Caching
{
    /// <inheritdoc cref="IGenericCache" />
    public class GenericCache : IGenericCache
    {
        // GenericCacheKeyBase is a record so a custom comparer for the dictionary is not required
        private readonly ConcurrentDictionary<GenericCacheKeyBase, object> _cache = new();

        /// <summary>A static instance of a <see cref="GenericCache"/>.</summary>
        public static readonly GenericCache Default = [];

        /// <summary>The number of elements in the cache.</summary>
        public int Count => _cache.Count;

        /// <inheritdoc />
        public ICollection<GenericCacheKeyBase> Keys => _cache.Keys;

        /// <inheritdoc />
        public ICollection<object> Values => _cache.Values;

        /// <summary>Gets or sets an item in the cache.</summary>
        /// <param name="key">The key to get or set a value.</param>
        /// <returns>When reading, the value associated with the key.</returns>
        public object this[GenericCacheKeyBase key]
        {
            get => _cache[key];
            set => _cache[key] = value;
        }

        /// <summary>A helper method to create a generic cache key based on 1 key type.</summary>
        /// <typeparam name="TKey1">The key type.</typeparam>
        /// <param name="key1">The key value.</param>
        /// <returns>The generic cache key.</returns>
        public static GenericCacheKeyBase CreateKey<TKey1>(TKey1 key1)
        {
            return new GenericCacheKey<TKey1>(key1);
        }

        /// <summary>A helper method to create a generic cache key based on 2 key types.</summary>
        /// <typeparam name="TKey1">The first key type.</typeparam>
        /// <typeparam name="TKey2">The second key type.</typeparam>
        /// <param name="key1">The first key value.</param>
        /// <param name="key2">The second key value.</param>
        /// <returns>The generic cache key.</returns>
        public static GenericCacheKeyBase CreateKey<TKey1, TKey2>(TKey1 key1, TKey2 key2)
        {
            return new GenericCacheKey<TKey1, TKey2>(key1, key2);
        }

        /// <summary>A helper method to create a generic cache key based on 3 key types.</summary>
        /// <typeparam name="TKey1">The first key type.</typeparam>
        /// <typeparam name="TKey2">The second key type.</typeparam>
        /// <typeparam name="TKey3">The third key type.</typeparam>
        /// <param name="key1">The first key value.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key3">The third key value.</param>
        /// <returns>The generic cache key.</returns>
        public static GenericCacheKeyBase CreateKey<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return new GenericCacheKey<TKey1, TKey2, TKey3>(key1, key2, key3);
        }

        /// <summary>A helper method to create a generic cache key based on 4 key types.</summary>
        /// <typeparam name="TKey1">The first key type.</typeparam>
        /// <typeparam name="TKey2">The second key type.</typeparam>
        /// <typeparam name="TKey3">The third key type.</typeparam>
        /// <typeparam name="TKey4">The fourth key type.</typeparam>
        /// <param name="key1">The first key value.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key3">The third key value.</param>
        /// <param name="key4">The fourth key value.</param>
        /// <returns>The generic cache key.</returns>
        public static GenericCacheKeyBase CreateKey<TKey1, TKey2, TKey3, TKey4>(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4)
        {
            return new GenericCacheKey<TKey1, TKey2, TKey3, TKey4>(key1, key2, key3, key4);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<GenericCacheKeyBase, object>> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        /// <summary>Determines if the cache contains the provided key.</summary>
        /// <param name="key">The key to lookup.</param>
        /// <returns><see langword="true" /> if the cache contains the key, otherwise <see langword="false" />.</returns>
        public bool ContainsKey(GenericCacheKeyBase key)
        {
            _ = key.WhenNotNull(nameof(key));

            return _cache.ContainsKey(key);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _cache.Clear();
        }

        /// <inheritdoc />
        public void Add<TValue>(GenericCacheKeyBase key, TValue value)
        {
            _ = key.WhenNotNull(nameof(key));

            ((IDictionary<GenericCacheKeyBase, object>) _cache).Add(key, value);
        }

        /// <inheritdoc />
        public bool TryAdd<TValue>(GenericCacheKeyBase key, TValue value)
        {
            _ = key.WhenNotNull(nameof(key));

            return _cache.TryAdd(key, value);
        }

        /// <summary>Attempts to get the value associated with a key in the cache.</summary>
        /// <param name="key">The custom key associated with the value.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns><see langword="true" /> if the key was found in the cache, otherwise <see langword="false" />.</returns>
        public bool TryGetValue(GenericCacheKeyBase key, out object value)
        {
            _ = key.WhenNotNull(nameof(key));

            return _cache.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public bool TryGetValue<TValue>(GenericCacheKeyBase key, out TValue value)
        {
            _ = key.WhenNotNull(nameof(key));

            var success = _cache.TryGetValue(key, out var keyValue);

            value = success
                ? (TValue) keyValue
                : default;

            return success;
        }

        /// <summary>Attempts to remove a key and its associated value from the cache.</summary>
        /// <param name="item">The custom key and associated value.</param>
        /// <returns><see langword="true" /> if the key was found in the cache, otherwise <see langword="false" />.</returns>
        public bool Remove(KeyValuePair<GenericCacheKeyBase, object> item)
        {
            return ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(item);
        }

        /// <inheritdoc />
        public bool TryRemove<TValue>(GenericCacheKeyBase key, out TValue value)
        {
            _ = key.WhenNotNull(nameof(key));

            var success = _cache.TryRemove(key, out var keyValue);

            value = success
                ? (TValue) keyValue
                : default;

            return success;
        }

#if !NETSTANDARD2_1
        /// <inheritdoc />
        public bool TryRemove<TValue>(KeyValuePair<GenericCacheKeyBase, TValue> item)
        {
            var keyValue = new KeyValuePair<GenericCacheKeyBase, object>(item.Key, item.Value);

            return _cache.TryRemove(keyValue);
        }
#endif

        /// <inheritdoc />
        public bool TryUpdate<TValue>(GenericCacheKeyBase key, TValue newValue, TValue comparisonValue)
        {
            _ = key.WhenNotNull(nameof(key));

            return _cache.TryUpdate(key, newValue, comparisonValue);
        }

        /// <inheritdoc />
        public KeyValuePair<GenericCacheKeyBase, object>[] ToArray()
        {
            return [.. _cache];
        }

        /// <inheritdoc />
        public TValue GetOrAdd<TValue>(GenericCacheKeyBase key, Func<GenericCacheKeyBase, TValue> addResolver)
        {
            _ = key.WhenNotNull(nameof(key));
            _ = addResolver.WhenNotNull(nameof(addResolver));

            return (TValue) _cache.GetOrAdd(key, valueKey => addResolver.Invoke(valueKey));
        }

        /// <inheritdoc />
        public TValue GetOrAdd<TValue>(GenericCacheKeyBase key, TValue value)
        {
            _ = key.WhenNotNull(nameof(key));

            return (TValue) _cache.GetOrAdd(key, value);
        }

        /// <inheritdoc />
        public TValue GetOrAdd<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg, TValue> addResolver,
            TArg resolverArgument)
        {
            _ = key.WhenNotNull(nameof(key));
            _ = addResolver.WhenNotNull(nameof(addResolver));

            object objectResolver(GenericCacheKeyBase valueKey, TArg arg) => addResolver.Invoke(valueKey, arg);

            return (TValue) _cache.GetOrAdd(
                key,
                objectResolver,
                resolverArgument);
        }

        /// <inheritdoc />
        public TValue AddOrUpdate<TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TValue> addResolver,
            Func<GenericCacheKeyBase, TValue, TValue> updateResolver)
        {
            _ = key.WhenNotNull(nameof(key));
            _ = addResolver.WhenNotNull(nameof(addResolver));
            _ = updateResolver.WhenNotNull(nameof(updateResolver));

            object objectAddResolver(GenericCacheKeyBase valueKey) => addResolver.Invoke(valueKey);

            object objectUpdateResolver(GenericCacheKeyBase valueKey, object value) => updateResolver.Invoke(valueKey, (TValue) value);

            return (TValue) _cache.AddOrUpdate(
                key,
                objectAddResolver,
                objectUpdateResolver);
        }

        /// <inheritdoc />
        public TValue AddOrUpdate<TValue>(
            GenericCacheKeyBase key,
            TValue addValue,
            Func<GenericCacheKeyBase, TValue, TValue> updateResolver)
        {
            _ = key.WhenNotNull(nameof(key));
            _ = updateResolver.WhenNotNull(nameof(updateResolver));

            object objectUpdateResolver(GenericCacheKeyBase valueKey, object value) => updateResolver.Invoke(valueKey, (TValue) value);

            return (TValue) _cache.AddOrUpdate(
                key,
                addValue,
                objectUpdateResolver);
        }

        /// <inheritdoc />
        public TValue AddOrUpdate<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg, TValue> addResolver,
            Func<GenericCacheKeyBase, TValue, TArg, TValue> updateResolver,
            TArg resolverArgument)
        {
            _ = key.WhenNotNull(nameof(key));
            _ = addResolver.WhenNotNull(nameof(addResolver));
            _ = updateResolver.WhenNotNull(nameof(updateResolver));

            object objectAddResolver(GenericCacheKeyBase valueKey, TArg arg) => addResolver.Invoke(valueKey, arg);

            object objectUpdateResolver(GenericCacheKeyBase valueKey, object value, TArg arg) => updateResolver.Invoke(valueKey, (TValue) value, arg);

            return (TValue) _cache.AddOrUpdate(
                key,
                objectAddResolver,
                objectUpdateResolver,
                resolverArgument);
        }

        #region Explicit implementations
        bool ICollection<KeyValuePair<GenericCacheKeyBase, object>>.IsReadOnly => false;
        IEnumerable<GenericCacheKeyBase> IReadOnlyDictionary<GenericCacheKeyBase, object>.Keys => ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).Keys;
        IEnumerable<object> IReadOnlyDictionary<GenericCacheKeyBase, object>.Values => ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).Values;

        void ICollection<KeyValuePair<GenericCacheKeyBase, object>>.Add(KeyValuePair<GenericCacheKeyBase, object> item)
        {
            ((IDictionary<GenericCacheKeyBase, object>) _cache).Add(item);
        }

        void IDictionary<GenericCacheKeyBase, object>.Add(GenericCacheKeyBase key, object value)
        {
            _ = key.WhenNotNull(nameof(key));

            ((IDictionary<GenericCacheKeyBase, object>) _cache).Add(key, value);
        }

        bool ICollection<KeyValuePair<GenericCacheKeyBase, object>>.Contains(KeyValuePair<GenericCacheKeyBase, object> item)
        {
            return ((IDictionary<GenericCacheKeyBase, object>) _cache).Contains(item);
        }

        void ICollection<KeyValuePair<GenericCacheKeyBase, object>>.CopyTo(KeyValuePair<GenericCacheKeyBase, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<GenericCacheKeyBase, object>>) _cache).CopyTo(array, arrayIndex);
        }

        bool IDictionary<GenericCacheKeyBase, object>.Remove(GenericCacheKeyBase key)
        {
            _ = key.WhenNotNull(nameof(key));

            return ((IDictionary<GenericCacheKeyBase, object>) _cache).Remove(key);
        }

        bool IDictionary<GenericCacheKeyBase, object>.TryGetValue(GenericCacheKeyBase key, out object value)
        {
            _ = key.WhenNotNull(nameof(key));

            return ((IDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out value);
        }

        bool IReadOnlyDictionary<GenericCacheKeyBase, object>.TryGetValue(GenericCacheKeyBase key, out object value)
        {
            _ = key.WhenNotNull(nameof(key));

            return ((IReadOnlyDictionary<GenericCacheKeyBase, object>) _cache).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _cache).GetEnumerator();
        }

        #endregion
    }
}