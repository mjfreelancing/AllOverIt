﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Collections
{
    /// <summary>Provides a truly immutable dictionary.</summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
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
            : this([])
        {
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
            : this((IEnumerable<KeyValuePair<TKey, TValue>>) data)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="data">Data to be added to the readonly dictionary.</param>
        public ReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> data)
        {
            _dictionary = new Dictionary<TKey, TValue>(data);
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
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8767  // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767  // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
