using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Reflection;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AllOverIt.Converters
{
    /// <summary>A type converter that converts an <seealso cref="EnrichedEnum{TEnum}"/> to and from string or integral values.</summary>
    /// <typeparam name="TEnum">The <seealso cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public class EnrichedEnumTypeConverter<TEnum> : TypeConverter
        where TEnum : EnrichedEnum<TEnum>
    {
        private static readonly Type EnrichedEnumTypeConverterType = typeof(EnrichedEnumTypeConverter<TEnum>);

        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == CommonTypes.StringType || sourceType.IsIntegralType();
        }

        /// <inheritdoc />
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            return value.GetType() == CommonTypes.StringType
                ? EnrichedEnum<TEnum>.From((string) value)
                : EnrichedEnum<TEnum>.From(Convert.ToInt32(value));
        }

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
        {
            return destinationType is not null && (destinationType == CommonTypes.StringType || destinationType.IsIntegralType());
        }

        /// <inheritdoc />
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value == null)
            {
                return null;
            }

            return destinationType == CommonTypes.StringType
                ? ((EnrichedEnum<TEnum>) value).Name
                : Convert.ChangeType(((EnrichedEnum<TEnum>) value).Value, destinationType);
        }

        /// <summary>Creates a <see cref="TypeConverter"/> for an <see cref="EnrichedEnum{TEnum}"/> type.</summary>
        /// <returns>A new TypeConverter instance.</returns>
        public static TypeConverter Create()
        {
            return (TypeConverter) Activator.CreateInstance(EnrichedEnumTypeConverterType)!;
        }
    }
}