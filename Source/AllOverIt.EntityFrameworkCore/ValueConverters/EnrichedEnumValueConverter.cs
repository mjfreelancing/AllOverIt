using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AllOverIt.EntityFrameworkCore.ValueConverters
{
    /// <summary>Defines the conversion from an <see cref="EnrichedEnum{TEnum}"/> type to an integer.</summary>
    /// <typeparam name="TEnum">The enriched enum type. This must inherit <see cref="EnrichedEnum{TEnum}"/>.</typeparam>
    public sealed class EnrichedEnumValueConverter<TEnum> : ValueConverter<TEnum, int>
        where TEnum : EnrichedEnum<TEnum>
    {
        /// <summary>Constructor.</summary>
        public EnrichedEnumValueConverter()
            : base(item => item.Value, value => EnrichedEnum<TEnum>.From(value))
        {
        }
    }
}