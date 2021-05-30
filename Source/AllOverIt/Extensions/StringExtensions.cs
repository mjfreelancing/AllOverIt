using System;
using System.ComponentModel;

namespace AllOverIt.Extensions
{
    public static class StringExtensions
    {
        /// <summary>Converts a given string to another type.</summary>
        /// <typeparam name="TType">The type to convert to.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <param name="defaultValue">The value to return if <see cref="value"/> is null, empty or contains whitespace, or is considered
        /// invalid for the <typeparamref name="TType"/> converter.</param>
        /// <returns>The converted value, or the <see cref="defaultValue"/> value if the conversion cannot be performed.</returns>
        /// <remarks>
        ///   <para>Supported conversions include byte, sbyte, decimal, double, float, int, uint, long, ulong, short, ushort, string, bool and enum.</para>
        ///   <para>Char and Boolean type conversions must be performed using the <see cref="ObjectExtensions.As{TType}(object, TType)"/> method.</para>
        ///   <para>No attempt is made to avoid overflow or argument exceptions.</para>
        /// </remarks>
        public static TType As<TType>(this string value, TType defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            if (typeof(TType).IsEnum)
            {
                return (TType)Enum.Parse(typeof(TType), value, true);
            }

            if (typeof(TType) == typeof(bool))
            {
                switch (value)
                {
                    case "0": return (TType)Convert.ChangeType(false, typeof(TType));
                    case "1": return (TType)Convert.ChangeType(true, typeof(TType));

                        // fall through - true / false values will be converted via the type converter
                }
            }

            // perform this after the enum conversion attempt
            if (!TypeDescriptor.GetConverter(typeof(TType)).IsValid(value))
            {
                throw new ArgumentException($"No converter exists for type '{typeof(TType).Name}' when value = '{value}'.");
            }

            // will throw NotSupportedException if the conversion cannot be performed
            var converted = TypeDescriptor.GetConverter(typeof(TType)).ConvertFromString(value);

            return (TType)converted;
        }

        /// <summary>Converts a given string to another nullable type.</summary>
        /// <typeparam name="TType">The nullable type to convert to.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value, or null if the conversion cannot be performed.</returns>
        public static TType? AsNullable<TType>(this string value)
          where TType : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (typeof(TType).IsEnum)
            {
                // will throw ArgumentException is 'ignoreCase = false' and the value cannot be found
                return (TType)Enum.Parse(typeof(TType), value, true);
            }

            // perform this after the enum conversion attempt
            if (!TypeDescriptor.GetConverter(typeof(TType)).IsValid(value))
            {
                return null;
            }

            return As<TType>(value, default);
        }

        /// <summary>Determines if a string is null, empty, or contains whitespace.</summary>
        /// <param name="value">The string value to compare.</param>
        /// <returns>True if the string is null, empty, or contains whitespace, otherwise false.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
