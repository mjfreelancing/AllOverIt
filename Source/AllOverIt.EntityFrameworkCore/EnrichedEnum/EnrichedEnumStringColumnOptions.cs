using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    /// <summary>Provides string column options for configuring <see cref="EnrichedEnum{TEnum}"/> value conversion on a <see cref="ModelBuilder"/>.</summary>
    public record EnrichedEnumStringColumnOptions : EnrichedEnumColumnOptions
    {
        /// <summary>When not <see langword="null"/>, this value specifies the column's maximum length.
        /// This option is not required if the [MaxLength] attribute is used.</summary>
        public int? MaxLength { get; init; }
    }
}