using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    /// <summary>Provides string column options for configuring <see cref="EnrichedEnum{TEnum}"/> value conversion on a <see cref="ModelBuilder"/>.</summary>
    [ExcludeFromCodeCoverage]   // Does have tests related to checking properties, but these don't show up in the code coverage report
    public record EnrichedEnumStringColumnOptions : EnrichedEnumColumnOptions
    {
        /// <summary>When not <see langword="null"/>, this value specifies the column's maximum length.
        /// This option is not required if the [MaxLength] attribute is used.</summary>
        public int? MaxLength { get; init; }
    }
}
