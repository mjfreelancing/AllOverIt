using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Helpers
{
    /// <summary>Provides helper methods to assist with calculating hash codes from one or more values.</summary>
    public static class HashCodeHelper
    {
        /// <summary>Calculates a hash code based on the provided items.</summary>
        /// <param name="items">The items to calculate a hash code for.</param>
        /// <returns>A hash code based on the provided items.</returns>
        public static int CalculateHashCode(params object[] items)
        {
            return CalculateHashCode(17, items);
        }

        /// <summary>Calculates a hash code based on the provided items using a specified initial seed.</summary>
        /// <param name="seed">The initial seed value for the hash code.</param>
        /// <param name="items">The items to calculate a hash code for.</param>
        /// <returns>A hash code based on the provided items and specified initial seed.</returns>
        public static int CalculateHashCode(int seed, params object[] items)
        {
            return items.Aggregate(seed, (current, item) => current * 23 + (item?.GetHashCode() ?? 0));
        }

        // While this prevents boxing via 'params object[]' overload when there is a single item, there's too
        // much risk of of this overload being called for an IEnumerable<TType> (unless the call is explicitly
        // made with the generic type included.
        //
        // public static int CalculateHashCode<TType>(TType item)
        // {
        //     return CalculateHashCode(17, new[] {item});
        // }

        /// <summary>Calculates a hash code based on the provided items.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The items to calculate a hash code for.</param>
        /// <returns>A hash code based on the provided items.</returns>
        public static int CalculateHashCode<TType>(IEnumerable<TType> items)
        {
            return CalculateHashCode(17, items);
        }

        /// <summary>Calculates a hash code based on the provided items using a specified initial seed.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="seed">The initial seed value for the hash code.</param>
        /// <param name="items">The items to calculate a hash code for.</param>
        /// <returns>A hash code based on the provided items and specified initial seed.</returns>
        public static int CalculateHashCode<TType>(int seed, IEnumerable<TType> items)
        {
            return items.Aggregate(seed, (current, item) => current * 23 + (item?.GetHashCode() ?? 0));
        }
    }
}