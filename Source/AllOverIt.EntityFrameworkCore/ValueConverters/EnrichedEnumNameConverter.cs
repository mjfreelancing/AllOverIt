using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AllOverIt.EntityFrameworkCore.ValueConverters
{
    public class EnrichedEnumNameConverter<TEnum> : ValueConverter<TEnum, string>
        where TEnum : EnrichedEnum<TEnum>
    {
        public EnrichedEnumNameConverter()
            : base(item => item.Name, value => EnrichedEnum<TEnum>.From(value))
        {
        }
    }
}