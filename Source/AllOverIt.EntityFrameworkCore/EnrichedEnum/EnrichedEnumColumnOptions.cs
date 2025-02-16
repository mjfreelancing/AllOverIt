using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    /// <summary>Provides column options for configuring <see cref="EnrichedEnum{TEnum}"/> value conversion on a <see cref="ModelBuilder"/>.</summary>    
    [ExcludeFromCodeCoverage]   // Does have tests related to checking properties, but these don't show up in the code coverage report
    public record EnrichedEnumColumnOptions
    {
        /// <summary>When not <see langword="null"/>, this contains the data type of the column that the property maps to when targeting a relational database.
        /// This must be the complete type name applicable for the database in use.</summary>
        public string? ColumnType { get; init; }
    }
}