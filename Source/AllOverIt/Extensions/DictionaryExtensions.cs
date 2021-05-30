using System;
using System.Collections.Generic;

namespace AllOverIt.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            return dictionary.TryGetValue(key, out var result) ? result : defaultValue;
        }

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