using System.Collections.Generic;

namespace AllOverIt.Collections
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>Provides static methods related to list types.</summary>
    public static class List
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class EmptyReadOnlyList<TType>
        {
            internal static readonly IReadOnlyList<TType> Instance = new ReadOnlyList<TType>();
        }

        /// <summary>Gets a static instance of a <see cref="IReadOnlyList{T}"/>.</summary>
        /// <typeparam name="TType">The type associated with the list.</typeparam>
        /// <returns>A static empty list as an <see cref="IReadOnlyList{T}"/>.</returns>
        public static IReadOnlyList<TType> EmptyReadOnly<TType>()
        {
            return EmptyReadOnlyList<TType>.Instance;
        }
    }
}
