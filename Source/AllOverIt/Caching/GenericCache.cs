using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AllOverIt.Assertion;

namespace AllOverIt.Caching
{
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

        public static GenericCache Default = new();

        public int Count => _cache.Count;

        public ICollection<GenericCacheKeyBase> Keys => _cache.Keys;

        public ICollection<object> Values => _cache.Values;

        public object this[GenericCacheKeyBase key]
        {
            get => _cache[key];
            set => _cache[key] = value;
        }

        public IEnumerator<KeyValuePair<GenericCacheKeyBase, object>> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        public bool ContainsKey(GenericCacheKeyBase key)
        {
            _ = key.WhenNotNull(nameof(key));

            return _cache.ContainsKey(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }

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

        /// <inheritdoc />
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

#if !NETSTANDARD2_0
        /// <inheritdoc />
        public TValue GetOrAdd<TArg, TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TArg, TValue> addResolver,
            TArg resolverArgument)
        {
            _ = key.WhenNotNull(nameof(key));
            _ = addResolver.WhenNotNull(nameof(addResolver));

            Func<GenericCacheKeyBase, TArg, object> objectResolver = (valueKey, arg) => addResolver.Invoke(valueKey, arg);

            return (TValue) _cache.GetOrAdd(
                key,
                objectResolver,
                resolverArgument);
        }
#endif

        /// <inheritdoc />
        public TValue AddOrUpdate<TValue>(
            GenericCacheKeyBase key,
            Func<GenericCacheKeyBase, TValue> addResolver,
            Func<GenericCacheKeyBase, TValue, TValue> updateResolver)
        {
            _ = key.WhenNotNull(nameof(key));
            _ = addResolver.WhenNotNull(nameof(addResolver));
            _ = updateResolver.WhenNotNull(nameof(updateResolver));

            Func<GenericCacheKeyBase, object> objectAddResolver = valueKey => addResolver.Invoke(valueKey);
            Func<GenericCacheKeyBase, object, object> objectUpdateResolver = (valueKey, value) => updateResolver.Invoke(valueKey, (TValue) value);

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

            Func<GenericCacheKeyBase, object, object> objectUpdateResolver = (valueKey, value) => updateResolver.Invoke(valueKey, (TValue) value);

            return (TValue) _cache.AddOrUpdate(
                key,
                addValue,
                objectUpdateResolver);
        }

#if !NETSTANDARD2_0
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

            Func<GenericCacheKeyBase, TArg, object> objectAddResolver = (valueKey, arg) => addResolver.Invoke(valueKey, arg);
            Func<GenericCacheKeyBase, object, TArg, object> objectUpdateResolver = (valueKey, value, arg) => updateResolver.Invoke(valueKey, (TValue) value, arg);

            return (TValue) _cache.AddOrUpdate(
                key,
                objectAddResolver,
                objectUpdateResolver,
                resolverArgument);
        }
#endif

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