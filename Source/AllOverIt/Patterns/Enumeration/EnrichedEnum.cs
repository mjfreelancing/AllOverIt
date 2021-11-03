using AllOverIt.Assertion;
using AllOverIt.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Patterns.Enumeration
{
    // Inspired by https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    public abstract class EnrichedEnum<TEnum> : IComparable<EnrichedEnum<TEnum>>, IEquatable<EnrichedEnum<TEnum>>
        where TEnum : EnrichedEnum<TEnum>
    {
        private static readonly TEnum[] AllValues = GetAllEnums().ToArray();

        public int Value { get; }
        public string Name { get; }

        protected EnrichedEnum(int value, string name)
        {
            Value = value;
            Name = name.WhenNotNullOrEmpty(nameof(name));
        }

        public override string ToString() => Name;

        public virtual int CompareTo(EnrichedEnum<TEnum> other)
        {
            var value = other.WhenNotNull(nameof(other)).Value;
            
            return Value.CompareTo(value);
        }

        public override bool Equals(object obj) => obj is EnrichedEnum<TEnum> other && Equals(other);

        public virtual bool Equals(EnrichedEnum<TEnum> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is not null && Value.Equals(other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static IEnumerable<int> GetAllValues()
        {
            return AllValues.Select(item => item.Value);
        }

        public static IEnumerable<string> GetAllNames()
        {
            return AllValues.Select(item => item.Name);
        }

        public static IEnumerable<TEnum> GetAll()
        {
            return AllValues;
        }

        public static TEnum From(int value)
        {
            return Parse(value, item => item.Value == value);
        }

        // will try and parse by the EnrichedEnum Name, then by its Value if 'value' can be converted to an integer.
        public static TEnum From(string value)
        {
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

        public static bool TryFromNameOrValue(string nameOrValue, out TEnum enumeration)
        {
            // assume parsing a name
            if (TryParse(item => item.Name.Equals(nameOrValue, StringComparison.OrdinalIgnoreCase), out enumeration))
            {
                return true;
            }

            // fall back to a number as a string
            return int.TryParse(nameOrValue, out var intValue) && TryParse(item => item.Value == intValue, out enumeration);
        }

        public static bool operator ==(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => !(left == right);

        public static bool operator >(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) > 0;

        public static bool operator >=(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) >= 0;

        public static bool operator <(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) < 0;

        public static bool operator <=(EnrichedEnum<TEnum> left, EnrichedEnum<TEnum> right) => left.CompareTo(right) <= 0;

        public static implicit operator int(EnrichedEnum<TEnum> smartEnum) => smartEnum.Value;

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
