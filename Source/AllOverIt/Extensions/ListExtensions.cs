using System.Collections.Generic;

namespace AllOverIt.Extensions
{
    // TODO: TBC - needs tests
    public static class ListExtensions
    {
        public static TType FirstElement<TType>(this IReadOnlyList<TType> items)
        {
            return items[0];
        }

        public static TType LastElement<TType>(this IReadOnlyList<TType> items)
        {
            return items[items.Count - 1];
        }
    }
}