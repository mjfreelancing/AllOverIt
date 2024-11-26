using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    internal sealed class ColumnDescriptor : IColumnDescriptor
    {
        public string ColumnName { get; }
        public string ColumnType { get; }
        public bool IsNullable { get; }
        public int? MaxLength { get; }
        public ConstraintType Constraint { get; } = ConstraintType.None;
        public PrincipalForeignKey[] ForeignKeyPrincipals { get; } = [];

        internal ColumnDescriptor(IProperty column)
        {
            ColumnName = column.Name;
            ColumnType = column.GetColumnType();
            IsNullable = column.IsColumnNullable();

            var maxLength = column.GetAnnotations().SingleOrDefault(annotation => annotation.Name == nameof(MaxLength));

            if (maxLength?.Value is not null)
            {
                MaxLength = (int)maxLength.Value;
            }

            if (column.IsPrimaryKey())
            {
                Constraint = ConstraintType.PrimaryKey;
            }
            else if (column.IsForeignKey())
            {
                Constraint = ConstraintType.ForeignKey;
                ForeignKeyPrincipals = GetForeignKeys(column);
            }
        }

        // Alternative factory method that can be used as a method group
        internal static ColumnDescriptor Create(IProperty column)
        {
            return new ColumnDescriptor(column);
        }

        private static PrincipalForeignKey[] GetForeignKeys(IProperty column)
        {
            var foreignKeys = new List<PrincipalForeignKey>();

            foreach (var foreignKey in column.GetContainingForeignKeys())
            {
                var principalEntity = foreignKey.PrincipalEntityType;

                var parentToChildNavigation = foreignKey.DependentToPrincipal?.Inverse;

                Throw<DiagramException>.WhenNull(
                    parentToChildNavigation,
                    $"A parent to child navigation property exists between {principalEntity.DisplayName()} and {column.DeclaringType.DisplayName()}, but not the reverse.");

                var isOneToMany = parentToChildNavigation.IsCollection;

                // TODO: Configure / handle composite keys
                var entityColumn = new PrincipalForeignKey
                {
                    Type = principalEntity.ClrType,
                    EntityName = principalEntity.GetTableName()!,
                    ColumnName = string.Join(", ", foreignKey.PrincipalKey.Properties.Select(property => property.Name)),
                    IsOneToMany = isOneToMany
                };

                foreignKeys.Add(entityColumn);
            }

            return [.. foreignKeys];
        }
    }
}