﻿using System;
using System.Collections.Generic;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IDictionary{TKey,TValue}"/>.</summary>
    public static class DictionaryExtensions
    {
        /// <summary>Gets the value based on a specified key, or the value type default if the key is not found.</summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The source dictionary.</param>
        /// <param name="key">The key value.</param>
        /// <param name="defaultValue">When provided, this will be returned as the default value if the key is not found in the dictionary.</param>
        /// <returns>The value associated with the specified key, otherwise a default value.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            return dictionary.TryGetValue(key, out var result) ? result : defaultValue;
        }

        /// <summary>Gets the value based on a specified key, or sets the value via a Func when the key is not found.</summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The source dictionary.</param>
        /// <param name="key">The key value.</param>
        /// <param name="valueCreator">The Func that provides the value to set when the key is not found.</param>
        /// <returns>The value based on a specified key, or the value returned by a specified Func when the key is not found.</returns>
        public static TValue GetOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueCreator)
        {
            if (dictionary.TryGetValue(key, out var result))
            {
                return result;
            }

            var value = valueCreator.Invoke();
            dictionary.Add(key, value);

            return value;
        }
    }
}