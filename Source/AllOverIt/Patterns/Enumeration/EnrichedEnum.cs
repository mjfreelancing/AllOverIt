using AllOverIt.Assertion;
using AllOverIt.Patterns.Enumeration.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Patterns.Enumeration
{
    // Inspired by https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types

    /// <summary>A base class for creating an enumeration type that has a integer and string value.</summary>
    /// <typeparam name="TEnum">The type inheriting this class.</typeparam>
    public abstract class EnrichedEnum<TEnum> : IComparable<EnrichedEnum<TEnum>>, IEquatable<EnrichedEnum<TEnum>>
        where TEnum : EnrichedEnum<TEnum>
    {
        private static readonly TEnum[] AllValues = GetAllEnums().ToArray();

        /// <summary>The integer value of the enumeration.</summary>
        public int Value { get; }

        /// <summary>The name, or string value, of the enumeration.</summary>
        public string Name { get; }

        /// <summary>Constructor.</summary>
        [ExcludeFromCodeCoverage]
        protected EnrichedEnum()
        {
            // Required for some serialization scenarios
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">The integer value of the enumeration.</param>
        /// <param name="name">The name, or string value, of the enumeration.</param>
        protected EnrichedEnum(int value, string name)
        {
            Value = value;
            Name = name.WhenNotNullOrEmpty(nameof(name));
        }

        /// <summary>Returns a string representation of the enumeration. This will be the name value.</summary>
        public override string ToString() => Name;

        /// <inheritdoc />
        public int CompareTo(EnrichedEnum<TEnum> other)
        {
            var value = other.WhenNotNull(nameof(other)).Value;
            return Value.CompareTo(value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is EnrichedEnum<TEnum> other && Equals(other);

        /// <inheritdoc />
        public bool Equals(EnrichedEnum<TEnum> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is not null && Value.Equals(other.Value);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>Gets the integer values for all enumerations of this type.</summary>
        public static IEnumerable<int> GetAllValues()
        {
            return AllValues.Select(item => item.Value);
        }

        /// <summary>Gets the string values, or names, for all enumerations of this type.</summary>
        public static IEnumerable<string> GetAllNames()
        {
            return AllValues.Select(item => item.Name);
        }

        /// <summary>Gets all enumerations of this type.</summary>
        public static IEnumerable<TEnum> GetAll()
        {
            return AllValues;
        }

        /// <summary>Gets the enumeration instance that matches the provided integer value.</summary>
        /// <param name="value">The integer value of the enumeration instance to return.</param>
        /// <returns>The enumeration instance that matches the provided integer value.</returns>
        /// <exception cref="EnrichedEnumException">If the value does not match any of the enumeration instances.</exception>
        public static TEnum From(int value)
        {
            return Parse(value, item => item.Value == value);
        }

        /// <summary>Gets the enumeration instance that matches the provided string value, either by name or integer value.</summary>
        /// <param name="value">The integer value or name of the enumeration instance to return.</param>
        /// <returns>The enumeration instance that matches the provided value, either by name or integer value.</returns>
        /// <exception cref="EnrichedEnumException">If the name or integer value does not match any of the enumeration instances.</exception>
        public static TEnum From(string value)
        {
            _ = value.WhenNotNullOrEmpty(nameof(value));

            // assume parsing a name
            if (TryParse(item => item.Name.Equals(value, StringComparison.OrdinalIgnoreCase), out var enumeration))
            {
                return enumeration;
            }

            // see if the value is a string representation of an integer
            if (int.TryParse(value, out var intValue))
            {
                return From(intValue);
            }

            throw new EnrichedEnumException($"Unable to convert '{value}' to a {typeof(TEnum).Name}.");
        }

        /// <summary>Attempts to convert the provided value to an enumeration instance.</summary>
        /// <param name="value">The integer value to convert to an equivalent enumeration instance.</param>
        /// <param name="enumeration">The matching enumeration instance if a match is found.</param>
        /// <returns><see langword="true" /> if the conversion was successful, otherwise <see langword="false" />.</returns>
        public static bool TryFromValue(int value, out TEnum enumeration)
        {
            return TryParse(item => item.Value == value, out enumeration);
        }

        /// <summary>Attempts to convert the provided value to an enumeration instance.</summary>
        /// <param name="name">The name to convert to an equivalent enumeration instance.</param>
        /// <param name="enumeration">The matching enumeration instance if a match is found.</param>
        /// <returns><see langword="true" /> if the conversion was successful, otherwise <see langword="false" />.</returns>
        public static bool TryFromName(string name, out TEnum enumeration)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            return TryParse(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase), out enumeration);
        }

        /// <summary>Attempts to convert the provided value to an enumeration instance.</summary>
        /// <param name="nameOrValue">The string name, or integer value (as a string), to convert to an equivalent enumeration instance.</param>
        /// <param name="enumeration">The matching enumeration instance if a match is found.</param>
        /// <returns><see langword="true" /> if the conversion was successful, otherwise <see langword="false" />.</returns>
        public static bool TryFromNameOrValue(string nameOrValue, out TEnum enumeration)
        {
            _ = nameOrValue.WhenNotNullOrEmpty(nameof(nameOrValue));

            // assume parsing a name
            if (TryParse(item => item.Name.Equals(nameOrValue, StringComparison.OrdinalIgnoreCase), out enumeration))
            {
                return true;
            }

            // fall back to a number as a string
            return int.TryParse(nameOrValue, out var intValue) && TryParse(item => item.Value == intValue, out enumeration);
        }

        /// <summary>Indicates if the value matches one of the defined enumeration instances.</summary>
        /// <param name="value">The value to compare.</param>
        /// <returns><see langword="true" /> if the value matches one of the defined enumeration instances, otherwise <see langword="false" />.</returns>
        public static bool HasValue(int value)
        {
            return TryFromValue(value, out _);
        }

        /// <summary>Indicates if the name matches one of the defined enumeration instances.</summary>
        /// <param name="name">The value to compare.</param>
        /// <returns><see langword="true" /> if the name matches one of the defined enumeration instances, otherwise <see langword="false" />.</returns>
        public static bool HasName(string name)
        {
            return TryFromName(name, out _);
        }

        /// <summary>Indicates if the name or value (as a string) matches one of the defined enumeration instances.</summary>
        /// <param name="nameOrValue">The name or value to compare.</param>
        /// <returns><see langword="true" /> if the name or value matches one of the defined enumeration instances, otherwise <see langword="false" />.</returns>
        public static bool HasNameOrValue(string nameOrValue)
        {
            return TryFromNameOrValue(nameOrValue, out _);
        }

        // <summary>Operator that determines if two enumerations are equal.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns><see langword="true" /> if the enumerations are equal, otherwise <see langword="false" />.</returns>
        public static bool operator ==(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        // <summary>Operator that determines if two enumerations are not equal.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns><see langword="true" /> if the enumerations are not equal, otherwise <see langword="false" />.</returns>
        public static bool operator !=(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => !(left == right);

        // <summary>Operator that determines if one enumeration value is greater than another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns><see langword="true" /> if the left operand is greater than the right operand, otherwise <see langword="false" />.</returns>
        public static bool operator >(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) > 0;

        // <summary>Operator that determines if one enumeration value is greater than or equal to another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns><see langword="true" /> if the left operand is greater than or equal to the right operand, otherwise <see langword="false" />.</returns>
        public static bool operator >=(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) >= 0;

        // <summary>Operator that determines if one enumeration value is less than another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns><see langword="true" /> if the left operand is less than the right operand, otherwise <see langword="false" />.</returns>
        public static bool operator <(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) < 0;

        // <summary>Operator that determines if one enumeration value is less than or equal to another.</summary>
        /// <param name="left">The left operand of the comparison.</param>
        /// <param name="right">The right operand of the comparison.</param>
        /// <returns><see langword="true" /> if the left operand is less than or equal to the right operand, otherwise <see langword="false" />.</returns>
        public static bool operator <=(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) <= 0;

        /// <summary>Implicit operator to convert an enumeration to its integer equivalent.</summary>
        /// <param name="enum">The value to implicitly convert.</param>
        public static implicit operator int(EnrichedEnum<TEnum> @enum) => @enum.Value;

        /// <summary>Explicit operator to convert an integer value to its enumeration equivalent.</summary>
        /// <param name="value">The value to explicitly convert.</param>
        public static explicit operator EnrichedEnum<TEnum>(int value) => From(value);

        private static TEnum Parse<TValueType>(TValueType value, Func<TEnum, bool> predicate)
        {
            if (TryParse(predicate, out var enumeration))
            {
                return enumeration;
            }

            throw new EnrichedEnumException($"Unable to convert '{value}' to a {typeof(TEnum).Name}.");
        }

        private static bool TryParse(Func<TEnum, bool> predicate, out TEnum enumeration)
        {
            enumeration = AllValues.SingleOrDefault(predicate);
            return enumeration != null;
        }

        private static IEnumerable<TEnum> GetAllEnums()
        {
            var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields
                .Select(field => field.GetValue(null))
                .Cast<TEnum>();
        }
    }
}
