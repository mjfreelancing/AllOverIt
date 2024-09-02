namespace AllOverIt.Collections
{
    /// <summary>Provides static methods related to collection types.</summary>
    public static class Collection
    {
        private sealed class EmptyReadOnlyCollection<TType>
        {
            internal static readonly ReadOnlyCollection<TType> Instance = [];
        }

        /// <summary>Gets a static instance of a <see cref="ReadOnlyCollection{T}"/> that is empty and immutable.</summary>
        /// <typeparam name="TType">The type associated with the collection.</typeparam>
        /// <returns>A static empty, immutable, collection as a <see cref="ReadOnlyCollection{T}"/>.</returns>
        public static ReadOnlyCollection<TType> EmptyReadOnly<TType>()
        {
            return EmptyReadOnlyCollection<TType>.Instance;
        }
    }
}
