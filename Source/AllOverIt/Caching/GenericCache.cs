using AllOverIt.Assertion;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AllOverIt.Caching
{
    /// <summary>A cache capable of storing different key/value types. Each key type must inherit <see cref="GenericCacheKeyBase"/> and each
    /// of the key elements must support equality comparison.</summary>
    public class GenericCache : IGenericCache
    {
        private class GenericCacheKeyComparer : IEqualityComparer<GenericCacheKeyBase>
        {
            public static readonly GenericCacheKeyComparer Instance = new();

            public bool Equals(GenericCacheKeyBase x, GenericCacheKeyBase y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.Key.GetType() == y.Key.GetType() && x.Key.Equals(y.Key);
            }

            public int GetHashCode(GenericCacheKeyBase obj)
            {
                return obj.Key.GetHashCode();
            }
        }

        private readonly ConcurrentDictionary<GenericCacheKeyBase, object> _cache = new (GenericCacheKeyComparer.Instance);

        /// <summary>A static instance of a <see cref="GenericCache"/>.</summary>
        public static readonly GenericCache Default = new();

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

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<GenericCacheKeyBase, object>> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        /// <summary>Determines if the cache contains the provided key.</summary>
        /// <param name="key">The key to lookup.</param>
        /// <returns>True if the cache contains the key, otherwise false.</returns>
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
        /// <returns>True if the key was found in the cache, otherwise false.</returns>
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

        /// <summary>Removes a key from the cache if it was found.</summary>
        /// <param name="item">The custom key and associated value.</param>
        /// <returns>True if the key was found in the cache, otherwise false.</returns>
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

#if NET5_0_OR_GREATER
        // ReSharper disable once UseDeconstructionOnParameter (not available in all .NET versions)
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
            return _cache.ToArray();
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