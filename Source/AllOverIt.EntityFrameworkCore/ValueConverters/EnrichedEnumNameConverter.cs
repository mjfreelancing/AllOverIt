using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AllOverIt.EntityFrameworkCore.ValueConverters
{
    /// <summary>Defines the conversion from an <see cref="EnrichedEnum{TEnum}"/> type to a string.</summary>
    /// <typeparam name="TEnum">The enriched enum type. This must inherit <see cref="EnrichedEnum{TEnum}"/>.</typeparam>
    public sealed class EnrichedEnumNameConverter<TEnum> : ValueConverter<TEnum, string>
        where TEnum : EnrichedEnum<TEnum>
    {
        /// <summary>Constructor.</summary>
        public EnrichedEnumNameConverter()
            : base(item => item.Name, value => EnrichedEnum<TEnum>.From(value))
        {
        }
    }
}