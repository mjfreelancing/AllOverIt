using System;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for double values.</summary>
    public static class DoubleExtensions
    {
        private const double Epsilon = 1E-7;

        /// <summary>Determines if the provided value is considered to be zero.</summary>
        /// <param name="value">The value to be compared.</param>
        /// <returns>True if the value is considered to be zero, otherwise false.</returns>
        /// <remarks>The value is considered to be zero if the absolute value is less than an internal value of
        /// Epsilon (1E-7). Use the <see cref="IsZero(double, double)"/> overload to provide an alternative value
        /// of Epsilon.</remarks>
        public static bool IsZero(this double value)
        {
            return value.IsEqualTo(0.0d);
        }

        /// <summary>Determines if the provided value is considered to be zero based on a provided value of Epsilon.</summary>
        /// <param name="value">The value to be compared.</param>
        /// <param name="epsilon">The value of Epsilon to determine if the value should be considered as zero.</param>
        /// <returns>True if the value is considered to be zero, otherwise false.</returns>
        /// <remarks>The value is considered to be zero if the absolute value is less than the provided value of
        /// Epsilon.</remarks>
        public static bool IsZero(this double value, double epsilon)
        {
            return value.IsEqualTo(0.0d, epsilon);
        }

        /// <summary>Compares two double values and considers them equal if their absolute difference is less than an internal
        /// value of Epsilon (1E-7). Use the <see cref="IsEqualTo(double, double)"/> overload to provide an alternative value
        /// of Epsilon.</summary>
        /// <param name="lhs">The left double value to be compared.</param>
        /// <param name="rhs">The right double value to be compared.</param>
        /// <returns>True if the values are considered to be equal, otherwise false.</returns>
        public static bool IsEqualTo(this double lhs, double rhs)
        {
            return Math.Abs(lhs - rhs) < Epsilon;
        }

        /// <summary>Compares two double values and considers them equal if their absolute difference is less than the provided
        /// value of Epsilon.</summary>
        /// <param name="lhs">The left double value to be compared.</param>
        /// <param name="rhs">The right double value to be compared.</param>
        /// <param name="epsilon"></param>
        /// <returns>True if the values are considered to be equal, otherwise false.</returns>
        public static bool IsEqualTo(this double lhs, double rhs, double epsilon)
        {
            return Math.Abs(lhs - rhs) < epsilon;
        }
    }
}