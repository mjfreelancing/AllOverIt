using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AllOverIt.Collections
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>Provides static methods related to collection types.</summary>
    public sealed class Collection
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class EmptyReadOnlyCollection<TType>
        {
            internal static readonly IReadOnlyCollection<TType> Instance = new ReadOnlyCollection<TType>(new List<TType>());
        }

        /// <summary>Gets a static instance of a <see cref="IReadOnlyCollection{T}"/>.</summary>
        /// <typeparam name="TType">The type associated with the collection.</typeparam>
        /// <returns>A static empty collection as an <see cref="IReadOnlyCollection{T}"/>.</returns>
        public static IReadOnlyCollection<TType> EmptyReadOnly<TType>()
        {
            return EmptyReadOnlyCollection<TType>.Instance;
        }
    }
}
