using AllOverIt.Assertion;
using AllOverIt.Patterns.ValueObject.Exceptions;
using System;

namespace AllOverIt.Patterns.ValueObject
{
    /// <summary>Implements an immutable value object.</summary>
    /// <typeparam name="TValue">The type inheriting this class.</typeparam>
    /// <typeparam name="TType">The underlying storage type for the immutable value.</typeparam>
    public class ValueObject<TValue, TType> : IComparable<ValueObject<TValue, TType>>, IEquatable<ValueObject<TValue, TType>>
        where TType : ValueObject<TValue, TType>
        where TValue : IComparable<TValue>, IEquatable<TValue>
    {
        private TValue _value;

        /// <summary>The underlying value.</summary>
        public TValue Value
        {
            get => _value;

            protected set
            {
                AssertValue(value);
                _value = value;
            }
        }

        /// <summary>Constructor.</summary>
        protected ValueObject()
        {
            // Required for some serialization scenarios
        }

        /// <summary>Constructor.</summary>
        protected ValueObject(TValue value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ValueObject<TValue, TType> other && Equals(other);

        /// <inheritdoc />
        public bool Equals(ValueObject<TValue, TType> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is not null && Value.Equals(other.Value);
        }

        /// <inheritdoc />
        public int CompareTo(ValueObject<TValue, TType> other)
        {
            var value = other.WhenNotNull(nameof(other)).Value;
            return Value.CompareTo(value);
        }

        // <summary>Operator that determines if two ValueObject instances are equal.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns>True if the ValueObject instances are equal, otherwise false.</returns>
        public static bool operator ==(ValueObject<TValue, TType> left, ValueObject<TValue, TType> right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        // <summary>Operator that determines if two ValueObject instances are not equal.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns>True if the ValueObject instances are not equal, otherwise false.</returns>
        public static bool operator !=(ValueObject<TValue, TType> left, ValueObject<TValue, TType> right) => !(left == right);

        // <summary>Operator that determines if one ValueObject's value is greater than another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns>True if the left operand is greater than the right operand, otherwise false.</returns>
        public static bool operator >(ValueObject<TValue, TType> left, ValueObject<TValue, TType> right) => left.CompareTo(right) > 0;

        // <summary>Operator that determines if one ValueObject's value is greater than or equal to another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns>True if the left operand is greater than or equal to the right operand, otherwise false.</returns>
        public static bool operator >=(ValueObject<TValue, TType> left, ValueObject<TValue, TType> right) => left.CompareTo(right) >= 0;

        // <summary>Operator that determines if one ValueObject's value is less than another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns>True if the left operand is less than the right operand, otherwise false.</returns>
        public static bool operator <(ValueObject<TValue, TType> left, ValueObject<TValue, TType> right) => left.CompareTo(right) < 0;

        // <summary>Operator that determines if one ValueObject's value is less than or equal to another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns>True if the left operand is less than or equal to the right operand, otherwise false.</returns>
        public static bool operator <=(ValueObject<TValue, TType> left, ValueObject<TValue, TType> right) => left.CompareTo(right) <= 0;

        /// <summary>Implicit operator to convert a ValueObject to its underlying value equivalent.</summary>
        /// <param name="value">The value to implicitly convert.</param>
        public static implicit operator TValue(ValueObject<TValue, TType> value) => value.Value;

        /// <summary>Explicit operator to convert an underlying type to its ValueObject equivalent.</summary>
        /// <param name="value">The value to explicitly convert.</param>
        public static explicit operator ValueObject<TValue, TType>(TValue value) => new (value);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>Override to validate the initialized value.</summary>
        /// <param name="value">The value to be validated.</param>
        protected virtual bool ValidateValue(TValue value)
        {
            return true;
        }

        private void AssertValue(TValue value)
        {
            if (!ValidateValue(value))
            {
                throw new ValueObjectValidationException(value, $"Invalid value '{value}'.");
            }
        }
    }
}
