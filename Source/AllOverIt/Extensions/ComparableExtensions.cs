using System;

namespace AllOverIt.Extensions
{
    public static class ComparableExtensions
    {
        // Compares one comparable with another and returns a result indicating if this is less than the other.
        public static bool LessThan<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) < 0;
        }

        // Compares one comparable with another and returns a result indicating if this is less than or equal to the other.
        public static bool LessThanOrEqual<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) <= 0;
        }

        // Compares one comparable with another and returns a result indicating if this is greater than the other.
        public static bool GreaterThan<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) > 0;
        }

        // Compares one comparable with another and returns a result indicating if this is greater than or equal to the other.
        public static bool GreaterThanOrEqual<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) >= 0;
        }

        // Compares one comparable with another and returns a result indicating if this is equal to the other.
        public static bool EqualTo<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) == 0;
        }

        // Compares one comparable with another and returns a result indicating if this is not equal to the other.
        public static bool NotEqualTo<TType>(this IComparable<TType> comparable, TType other)
        {
            return comparable.CompareTo(other) != 0;
        }
    }
}