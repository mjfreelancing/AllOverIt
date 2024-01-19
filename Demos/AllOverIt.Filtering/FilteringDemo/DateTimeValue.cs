using System;

namespace FilteringDemo
{
    public sealed class DateTimeValue
    {
        public DateTime Value { get; set; }

        public static implicit operator DateTime?(DateTimeValue dateTimeValue) => dateTimeValue?.Value;
        public static explicit operator DateTimeValue(DateTime value) => new() { Value = value };

        // Used for custom value conversion when converting a LINQ expression to a query string
        public override string ToString() => Value.ToString();

        public static bool operator >=(DateTimeValue left, DateTimeValue right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return right is null;
            }

            return left.Value >= right.Value;
        }

        public static bool operator <=(DateTimeValue left, DateTimeValue right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return left is null;
            }

            return left.Value <= right.Value;
        }

        public static bool operator >(DateTimeValue left, DateTimeValue right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            if (left == null || right == null)
            {
                return right is null;
            }

            return left.Value > right.Value;
        }

        public static bool operator <(DateTimeValue left, DateTimeValue right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            if (left == null || right == null)
            {
                return left is null;
            }

            return left.Value < right.Value;
        }
    }
}