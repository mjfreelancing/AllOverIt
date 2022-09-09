using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Helpers
{
    /// <summary>Provides helper methods to assist with calculating hash codes from one or more values.</summary>
    public static class HashCodeHelper
    {
        /// <summary>Calculates a hash code based on the provided items using a specified initial seed.</summary>
        /// <param name="items">The items to calculate a hash code for.</param>
        /// <returns>A hash code based on the provided items and specified initial seed.</returns>
        public static int CalculateHashCode(params object[] items)
        {
            return CalculateHashCode<object>(items.ToArray());
        }

        /// <summary>Calculates a hash code based on the provided items.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The items to calculate a hash code for.</param>
        /// <returns>A hash code based on the provided items.</returns>
        public static int CalculateHashCode<TType>(IEnumerable<TType> items)
        {
            var hash = new HashCode();

            foreach (var item in items)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
    }
}