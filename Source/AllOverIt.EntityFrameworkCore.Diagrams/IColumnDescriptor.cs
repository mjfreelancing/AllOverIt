using System.Collections.Generic;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Describes an entity column.</summary>
    public interface IColumnDescriptor
    {
        /// <summary>The column name.</summary>
        string ColumnName { get; }

        /// <summary>The column type.</summary>
        string ColumnType { get; }

        /// <summary>Indicates if the column is nullable.</summary>
        bool IsNullable { get; }

        /// <summary>Indicates the column's maximum length, where applicable.</summary>
        int? MaxLength { get; }

        /// <summary>Indicates the constraint type.</summary>
        ConstraintType Constraint { get; }

        /// <summary>Provides foreign key principles.</summary>
        IReadOnlyCollection<PrincipalForeignKey> ForeignKeyPrincipals { get; }
    }
}