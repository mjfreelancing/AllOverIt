using AllOverIt.Assertion;
using AllOverIt.Reflection;
using System.ComponentModel;
using System.Text;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="string"/> types.</summary>
    public static class StringExtensions
    {
        /// <summary>Converts a given string to another type.</summary>
        /// <typeparam name="TType">The type to convert to.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value"/> is null, empty or contains whitespace, or is considered
        /// invalid for the <typeparamref name="TType"/> converter.</param>
        /// <returns>The converted value, or the <paramref name="defaultValue"/> value if the conversion cannot be performed.</returns>
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

            var valueType = typeof(TType);

            if (valueType.IsEnum)
            {
                return (TType) Enum.Parse(valueType, value, true);
            }

            if (valueType == CommonTypes.BoolType)
            {
                switch (value)
                {
                    case "0":
                        return (TType) Convert.ChangeType(false, valueType);

                    case "1":
                        return (TType) Convert.ChangeType(true, valueType);

                    default:
                        // fall through - true / false values will be converted via the type converter
                        break;
                }
            }

            // perform this after the enum conversion attempt
            var typeConverter = TypeDescriptor.GetConverter(valueType);

            if (!typeConverter.IsValid(value))
            {
                throw new ArgumentException($"No converter exists for type '{valueType.Name}' when value = '{value}'.");
            }

            // will throw NotSupportedException if the conversion cannot be performed
            var converted = typeConverter.ConvertFromString(value);

            return (TType) converted;
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
                return (TType) Enum.Parse(typeof(TType), value, true);
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
        /// <returns><see langword="true" /> if the string is null, empty, or contains whitespace, otherwise <see langword="false" />.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>Determines if a string is not null, empty, or containing whitespace.</summary>
        /// <param name="value">The string value to compare.</param>
        /// <returns><see langword="true" /> if the string is not null, not empty, nor contains whitespace, otherwise <see langword="false" />.</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>Determines if a string is not null and empty or contains whitespace.</summary>
        /// <param name="value">The string value to compare.</param>
        /// <returns><see langword="true" /> if the string is not null and empty or contains whitespace, otherwise <see langword="false" />.</returns>
        public static bool IsEmpty(this string value)
        {
            return value is not null && string.IsNullOrWhiteSpace(value);
        }

        /// <summary>Encodes a string value using base-64 digits.</summary>
        /// <param name="value">The value to convert to its string representation using base-64 digits.</param>
        /// <returns>A string encoded using base-64 digits that represents the source value.</returns>
        public static string ToBase64(this string value)
        {
            _ = value.WhenNotNull(nameof(value));

            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>Decodes a string previously encoded with base-64 digits.</summary>
        /// <param name="value">The base-64 representation of a string to convert.</param>
        /// <returns>The string decoded from a source string previously encoded with base-64 digits.</returns>
        public static string FromBase64(this string value)
        {
            _ = value.WhenNotNull(nameof(value));

            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>Copies, and returns, a specified string to a memory stream.</summary>
        /// <param name="value">The string content to copy to the memory stream.</param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this string value)
        {
            _ = value.WhenNotNull(nameof(value));

            var bytes = Encoding.UTF8.GetBytes(value);
            return new MemoryStream(bytes);
        }
    }
}
