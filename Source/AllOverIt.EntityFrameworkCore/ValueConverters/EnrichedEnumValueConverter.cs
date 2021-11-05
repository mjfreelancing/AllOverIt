using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AllOverIt.EntityFrameworkCore.ValueConverters
{
    public class EnrichedEnumValueConverter<TEnum> : ValueConverter<TEnum, int>
        where TEnum : EnrichedEnum<TEnum>
    {
        public EnrichedEnumValueConverter()
            : base(item => item.Value, value => EnrichedEnum<TEnum>.From(value))
        {
        }
    }
}