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

            // Check for foreign keys first, as a column can be both PK and FK (e.g., in join tables)
            if (column.IsForeignKey())
            {
                Constraint = ConstraintType.ForeignKey;
                ForeignKeyPrincipals = GetForeignKeys(column);
            }
            else if (column.IsPrimaryKey())
            {
                Constraint = ConstraintType.PrimaryKey;
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

                // For shadow entities (e.g., many-to-many join tables created with UsingEntity()),
                // there may not be navigation properties. In that case, we assume it's one-to-many.
                bool isOneToMany;

                if (parentToChildNavigation is not null)
                {
                    isOneToMany = parentToChildNavigation.IsCollection;
                }
                else
                {
                    // Shadow entity without navigation - assume one-to-many (typical for join tables)
                    isOneToMany = true;
                }

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