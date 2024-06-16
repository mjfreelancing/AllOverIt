using AllOverIt.Assertion;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IComparer{TType}"/> types.</summary>
    public static class ComparerExtensions
    {
        // Decorates an IComparer<TType> to negate its result
        private sealed class ReverseComparer<TType> : IComparer<TType>
        {
            private readonly IComparer<TType> _comparer;

            public ReverseComparer(IComparer<TType> comparer)
            {
                _comparer = comparer;
            }

            public int Compare(TType? lhs, TType? rhs)
            {
                // Not considering null lhs or rhs here since this should be handled by _comparer

#pragma warning disable IDE0079     // unnecessary suppression
#pragma warning disable CS8604      // Possible null refererence argument => Compare() allows null
                var result = _comparer.Compare(lhs, rhs);
#pragma warning restore CS8604
#pragma warning restore IDE0079

                return result == 0 ? result : -result;
            }
        }

        // Applies the chain of responsibility to a pair of IComparer<TType>
        private sealed class ComparerNode<TType> : IComparer<TType>
        {
            private readonly IComparer<TType> _first;
            private readonly IComparer<TType> _next;

            public ComparerNode(IComparer<TType> first, IComparer<TType> next)
            {
                _first = first;
                _next = next;
            }

            int IComparer<TType>.Compare(TType? lhs, TType? rhs)
            {
                // Not considering null lhs or rhs here since this should be handled by _first and _next respectively

#pragma warning disable IDE0079     // unnecessary suppression
#pragma warning disable CS8604      // Possible null refererence argument => Compare() allows null
                var result = _first.Compare(lhs, rhs);
#pragma warning restore CS8604
#pragma warning restore IDE0079

                return result != 0
                    ? result
                    : _next.Compare(lhs, rhs);
            }
        }

        /// <summary>Negates the result of comparing two objects, thereby reversing the order of compared objects.</summary>
        /// <typeparam name="TType">The object type being compared.</typeparam>
        /// <param name="comparer">The type comparer.</param>
        /// <returns>A new comparer that negates the result returned by <paramref name="comparer"/>.</returns>
        public static IComparer<TType> Reverse<TType>(this IComparer<TType> comparer)
        {
            _ = comparer.WhenNotNull(nameof(comparer));

            return new ReverseComparer<TType>(comparer);
        }

        /// <summary>Creates a comparer that compares an object using <paramref name="first"/>, then by <paramref name="next"/>
        /// if the first comparer indicated the objects were equal.</summary>
        /// <typeparam name="TType">The object type being compared.</typeparam>
        /// <param name="first">The first comparer.</param>
        /// <param name="next">The next comparer to be invoked if the <paramref name="first"/> indicated two objects were equal.</param>
        /// <returns>A new comparer that composes <paramref name="first"/> and <paramref name="next"/>.</returns>
        public static IComparer<TType> Then<TType>(this IComparer<TType> first, IComparer<TType> next)
        {
            _ = first.WhenNotNull(nameof(first));
            _ = next.WhenNotNull(nameof(next));

            return new ComparerNode<TType>(first, next);
        }
    }
}