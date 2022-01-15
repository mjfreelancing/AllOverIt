using System;
using System.ComponentModel;
using System.Globalization;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;

namespace AllOverIt.Converters
{
    /// <summary>A type converter that converts an <seealso cref="EnrichedEnum{TEnum}"/> to and from string or integral values.</summary>
    /// <typeparam name="TEnum">The <seealso cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public class EnrichedEnumTypeConverter<TEnum> : TypeConverter
        where TEnum : EnrichedEnum<TEnum>
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType.IsIntegralType();
        }

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            return value.GetType() == typeof(string)
                ? EnrichedEnum<TEnum>.From((string) value)
                : EnrichedEnum<TEnum>.From(Convert.ToInt32(value));
        }

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || destinationType.IsIntegralType();
        }

        /// <inheritdoc />
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
            {
                return null;
            }

            return destinationType == typeof(string)
                ? ((EnrichedEnum<TEnum>) value).Name
                : Convert.ChangeType(((EnrichedEnum<TEnum>) value).Value, destinationType);
        }
    }
}