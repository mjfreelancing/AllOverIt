using System;

namespace AllOverIt.Extensions
{
    public static class DoubleExtensions
    {
        private const double Epsilon = 1E-7;

        public static bool IsZero(this double value)
        {
            return value.IsEqualTo(0.0d);
        }

        public static bool IsZero(this double value, double tolerance)
        {
            return value.IsEqualTo(0.0d, tolerance);
        }

        public static bool IsEqualTo(this double lhs, double rhs)
        {
            return Math.Abs(lhs - rhs) < Epsilon;
        }

        public static bool IsEqualTo(this double lhs, double rhs, double tolerance)
        {
            return Math.Abs(lhs - rhs) < tolerance;
        }
    }
}