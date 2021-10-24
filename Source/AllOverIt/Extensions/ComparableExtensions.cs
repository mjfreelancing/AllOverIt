using System;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IComparable{TType}"/> types.</summary>
    public static class ComparableExtensions
    {
        /// <summary>Compares one comparable with another and returns a result indicating if it is less than the other.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="comparable">The left operand of the comparison.</param>
        /// <param name="other">The right operand of the comparison.</param>
        /// <returns>True if the left operand is less than the right operand.</returns>
        public static bool LessThan<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) < 0;
        }

        /// <summary>Compares one comparable with another and returns a result indicating if it is less than or equal to the other.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="comparable">The left operand of the comparison.</param>
        /// <param name="other">The right operand of the comparison.</param>
        /// <returns>True if the left operand is less than or equal to the right operand.</returns>
        public static bool LessThanOrEqual<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) <= 0;
        }

        /// <summary>Compares one comparable with another and returns a result indicating if it is greater than the other.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="comparable">The left operand of the comparison.</param>
        /// <param name="other">The right operand of the comparison.</param>
        /// <returns>True if the left operand is greater than the right operand.</returns>
        public static bool GreaterThan<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) > 0;
        }

        /// <summary>Compares one comparable with another and returns a result indicating if it is greater than or equal to the other.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="comparable">The left operand of the comparison.</param>
        /// <param name="other">The right operand of the comparison.</param>
        /// <returns>True if the left operand is greater than or equal to the right operand.</returns>
        public static bool GreaterThanOrEqual<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) >= 0;
        }

        /// <summary>Compares one comparable with another and returns a result indicating if it is equal to the other.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="comparable">The left operand of the comparison.</param>
        /// <param name="other">The right operand of the comparison.</param>
        /// <returns>True if the left operand is equal to the right operand.</returns>
        public static bool EqualTo<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) == 0;
        }

        /// <summary>Compares one comparable with another and returns a result indicating if it is not equal to the other.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="comparable">The left operand of the comparison.</param>
        /// <param name="other">The right operand of the comparison.</param>
        /// <returns>True if the left operand is not equal to the right operand.</returns>
        public static bool NotEqualTo<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) != 0;
        }
    }
}