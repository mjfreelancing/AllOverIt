namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Contains information for a principal (primary) key.</summary>
    public sealed class PrincipalForeignKey
    {
        /// <summary>The principle entity type.</summary>
        public required Type Type { get; init; }

        /// <summary>The name of the principal entity.</summary>
        public required string EntityName { get; init; }

        /// <summary>The name of the column on the principal entity.</summary>
        public required string ColumnName { get; init; }

        /// <summary>When <see langword="True"/>, indicates the foreign key is one-to-many, otherwise
        /// it is assumed to be one-to-one..</summary>
        public required bool IsOneToMany { get; init; }
    }
}