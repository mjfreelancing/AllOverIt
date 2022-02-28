using System.Collections.Generic;

namespace AllOverIt.Collections
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>Provides static methods related to dictionary types.</summary>
    public sealed class Dictionary
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class EmptyReadOnlyDictionary<TKey, TValue>
        {
            internal static readonly IReadOnlyDictionary<TKey, TValue> Instance = new Dictionary<TKey, TValue>();
        }

        /// <summary>Gets a static instance of a <see cref="IReadOnlyDictionary{TKey, TValue}"/>.</summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <returns>A static empty dictionary as an <see cref="IReadOnlyDictionary{TKey, TValue}"/>.</returns>
        public static IReadOnlyDictionary<TKey, TValue> EmptyReadOnly<TKey, TValue>()
        {
            return EmptyReadOnlyDictionary<TKey, TValue>.Instance;
        }
    }
}
