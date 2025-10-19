using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.EntityFrameworkCore.Diagrams.Exceptions;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    internal sealed class EntityNodeGenerator
    {
        private const string PrimaryKey = "primary_key";
        private const string ForeignKey = "foreign_key";

        private readonly ErdOptions _options;
        private readonly IEntityType[] _dbContextEntityTypes;
        private readonly string _defaultShapeStyle;

        public EntityNodeGenerator(ErdOptions options, IEntityType[] dbContextEntityTypes, string defaultShapeStyle)
        {
            _options = options.WhenNotNull();
            _dbContextEntityTypes = dbContextEntityTypes.WhenNotNullOrEmpty().AsArray();
            _defaultShapeStyle = defaultShapeStyle;     // can be string.Empty
        }

        public string CreateNode(EntityIdentifier entityIdentifier, IColumnDescriptor[] columns, Action<string> onRelationship)
        {
            _ = entityIdentifier.WhenNotNull();
            _ = columns.WhenNotNullOrEmpty();
            _ = onRelationship.WhenNotNull();

            var sb = new StringBuilder();

            var entityName = entityIdentifier.TableName;

            // Try to get group alias by type first, then by table name (for shadow entities)
            var groupAlias = _options.Groups.GetAlias(entityIdentifier.Type)
                             ?? _options.Groups.GetAlias(entityIdentifier.TableName);

            if (groupAlias is not null)
            {
                entityName = $"{groupAlias}.{entityName}";
            }

            sb.AppendLine($"{entityName}: {{");
            sb.AppendLine("  shape: sql_table");
            sb.AppendLine();

            // For shadow entities (e.g., many-to-many join tables created with UsingEntity()),
            // the ClrType is Dictionary<string, object> which is not unique.
            // We need to match by table name in those cases, so we may as always use this approach.
            var entityType = _dbContextEntityTypes
                .Single(entity => entity.GetTableName() == entityIdentifier.TableName)
                .ClrType;

            bool preserveColumnOrder;

            if (_options.TryGetEntityOptions(entityType, out var entityOptions))
            {
                preserveColumnOrder = entityOptions.PreserveColumnOrder;

                if (!entityOptions.ShapeStyle.IsDefault())
                {
                    sb.AppendLine(entityOptions.ShapeStyle.AsText(2));
                    sb.AppendLine();
                }
            }
            else
            {
                preserveColumnOrder = _options.Entities.PreserveColumnOrder;

                if (_defaultShapeStyle.IsNotNullOrEmpty())
                {
                    sb.AppendLine(_defaultShapeStyle);
                    sb.AppendLine();
                }
            }

            if (preserveColumnOrder)
            {
                columns = [.. GetPreservedColumnOrder(entityIdentifier, columns)];
            }

            foreach (var column in columns)
            {
                var columnName = column.ColumnName;
                var columnType = GetColumnDetail(entityType, column, _options);
                var columnConstraint = GetColumnConstraint(column);

                sb.AppendLine($"  {columnName}: {columnType} {columnConstraint}");

                if (column.ForeignKeyPrincipals is not null)
                {
                    var relationshipNodeGenerator = new RelationshipNodeGenerator(_options, _dbContextEntityTypes);

                    foreach (var foreignKey in column.ForeignKeyPrincipals)
                    {
                        var relationship = relationshipNodeGenerator.CreateNode(foreignKey, entityName, columnName);

                        onRelationship.Invoke(relationship);
                    }
                }
            }

            sb.Append('}');

            return sb.ToString();
        }

        private static IEnumerable<IColumnDescriptor> GetPreservedColumnOrder(EntityIdentifier entityIdentifier, IColumnDescriptor[] columns)
        {
            List<string> orderedPropertyNames = [];
            List<Type> orderedPropertyTypes = [];

            // Not using entityIdentifier.Type.GetProperties() as it returns base class properties after the derived type properties.
            foreach (var property in entityIdentifier.Type.GetPropertyInfo())
            {
                orderedPropertyNames.Add(property.Name);
                orderedPropertyTypes.Add(property.PropertyType);
            }

            return columns.OrderBy(column =>
            {
                var index = orderedPropertyNames.IndexOf(column.ColumnName);

                if (index != -1)
                {
                    return index;
                }

                if (column.ForeignKeyPrincipals.Length > 0)
                {
                    var foreignKey = column.ForeignKeyPrincipals.First();

                    return orderedPropertyTypes.IndexOf(foreignKey.Type);
                }

                return -1;
            });
        }

        private static string GetColumnDetail(Type entityType, IColumnDescriptor column, ErdOptions configuration)
        {
            _ = configuration.TryGetEntityOptions(entityType, out var options);

            ErdOptions.EntityOptionsBase entityOptions = options is not null
                ? options
                : configuration.Entities;

            var columnType = column.ColumnType;

            if (column.MaxLength.HasValue)
            {
                if (entityOptions.ShowMaxLength)
                {
                    columnType = $"{column.ColumnType}({column.MaxLength})";
                }
            }

            if (entityOptions.Nullable.IsVisible)
            {
                if (column.IsNullable && entityOptions.Nullable.Mode == NullableColumnMode.IsNull)
                {
                    columnType = $@"{columnType} {entityOptions.Nullable.IsNullLabel.D2EscapeString()}";
                }

                if (!column.IsNullable && entityOptions.Nullable.Mode == NullableColumnMode.NotNull)
                {
                    columnType = $@"{columnType} {entityOptions.Nullable.NotNullLabel.D2EscapeString()}";
                }
            }

            return columnType;
        }

        private static string GetColumnConstraint(IColumnDescriptor column)
        {
            return column.Constraint switch
            {
                ConstraintType.None => string.Empty,
                ConstraintType.PrimaryKey => $"{{ constraint: {PrimaryKey} }}",
                ConstraintType.ForeignKey => $"{{ constraint: {ForeignKey} }}",
                _ => throw new DiagramException($"Unhandled constraint type '{column.Constraint}'.")
            };
        }
    }
}