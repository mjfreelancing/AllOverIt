using System.Collections.Generic;

namespace AllOverIt.Collections
{
    /// <summary>Provides static methods related to dictionary types.</summary>
    public static class Dictionary
    {
        private sealed class EmptyReadOnlyDictionary<TKey, TValue>
            where TKey : notnull
        {
            internal static readonly IReadOnlyDictionary<TKey, TValue> Instance = new ReadOnlyDictionary<TKey, TValue>();
        }

        /// <summary>Gets a static instance of a <see cref="IReadOnlyDictionary{TKey, TValue}"/>.</summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <returns>A static empty dictionary as an <see cref="IReadOnlyDictionary{TKey, TValue}"/>.</returns>
        public static IReadOnlyDictionary<TKey, TValue> EmptyReadOnly<TKey, TValue>()
            where TKey : notnull
        {
            return EmptyReadOnlyDictionary<TKey, TValue>.Instance;
        }
    }
}
