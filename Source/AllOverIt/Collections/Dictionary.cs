namespace AllOverIt.Collections
{
    /// <summary>Provides static methods related to dictionary types.</summary>
    public static class Dictionary
    {
        private sealed class EmptyReadOnlyDictionary<TKey, TValue>
            where TKey : notnull
        {
            internal static readonly ReadOnlyDictionary<TKey, TValue> Instance = [];
        }

        /// <summary>Gets a static instance of a <see cref="ReadOnlyDictionary{TKey, TValue}"/>.</summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <returns>A static empty dictionary as a <see cref="ReadOnlyDictionary{TKey, TValue}"/>.</returns>
        public static ReadOnlyDictionary<TKey, TValue> EmptyReadOnly<TKey, TValue>()
            where TKey : notnull
        {
            return EmptyReadOnlyDictionary<TKey, TValue>.Instance;
        }
    }
}
