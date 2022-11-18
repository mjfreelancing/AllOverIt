using System.Collections.Generic;

namespace AllOverIt.Collections
{
    /// <summary>Provides static methods related to collection types.</summary>
    public static class Collection
    {
        private sealed class EmptyReadOnlyCollection<TType>
        {
            internal static readonly IReadOnlyCollection<TType> Instance = new ReadOnlyCollection<TType>();
        }

        /// <summary>Gets a static instance of a <see cref="IReadOnlyCollection{T}"/> that is empty and immutable.</summary>
        /// <typeparam name="TType">The type associated with the collection.</typeparam>
        /// <returns>A static empty, immutable, collection as an <see cref="IReadOnlyCollection{T}"/>.</returns>
        public static IReadOnlyCollection<TType> EmptyReadOnly<TType>()
        {
            return EmptyReadOnlyCollection<TType>.Instance;
        }
    }
}
