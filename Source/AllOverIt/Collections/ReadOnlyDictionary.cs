using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Collections
{
    /// <summary>Provides a truly immutable dictionary.</summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public sealed class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        /// <inheritdoc />
        public TValue this[TKey key] => _dictionary[key];

        /// <inheritdoc />
        public IEnumerable<TKey> Keys => _dictionary.Keys;

        /// <inheritdoc />
        public IEnumerable<TValue> Values => _dictionary.Values;

        /// <inheritdoc />
        public int Count => _dictionary.Count;

        /// <summary>Constructor.</summary>
        public ReadOnlyDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>Constructor.</summary>
        /// <param name="data">Data to be added to the readonly dictionary.</param>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> data)
        {
            _dictionary = new Dictionary<TKey, TValue>(data);
        }

        /// <summary>Constructor.</summary>
        /// <param name="data">Data to be added to the readonly dictionary.</param>
        public ReadOnlyDictionary(IReadOnlyDictionary<TKey, TValue> data)
            : this((IEnumerable<KeyValuePair<TKey, TValue>>)data)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="data">Data to be added to the readonly dictionary.</param>
        public ReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> data)
        {
#if NETSTANDARD2_0
            _dictionary = new Dictionary<TKey, TValue>();

            foreach (var item in data)
            {
                _dictionary[item.Key] = item.Value;
            }
#else
            _dictionary = new Dictionary<TKey, TValue>(data);
#endif
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
