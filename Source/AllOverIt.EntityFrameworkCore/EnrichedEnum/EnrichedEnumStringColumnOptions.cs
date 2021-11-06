using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    /// <summary>Provides string column options for configuring <see cref="EnrichedEnum{TEnum}"/> value conversion on a <see cref="ModelBuilder"/>.</summary>
    public record EnrichedEnumStringColumnOptions : EnrichedEnumColumnOptions
    {
        /// <summary>If provided this value specifies the column's maximum length. This parameter is not required if the [MaxLength] attribute is used.</summary>
        public int? MaxLength { get; init; }
    }
}