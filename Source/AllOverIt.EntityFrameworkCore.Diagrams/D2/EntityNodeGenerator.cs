﻿using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    internal sealed class EntityNodeGenerator
    {
        private const string PrimaryKey = "primary_key";
        private const string ForeignKey = "foreign_key";

        private readonly ErdOptions _options;
        private readonly IReadOnlyCollection<IEntityType> _dbContextEntityTypes;
        private readonly string _defaultShapeStyle;

        public EntityNodeGenerator(ErdOptions options, IReadOnlyCollection<IEntityType> dbContextEntityTypes, string defaultShapeStyle)
        {
            _options = options.WhenNotNull(nameof(options));
            _dbContextEntityTypes = dbContextEntityTypes.WhenNotNullOrEmpty(nameof(dbContextEntityTypes)) as IReadOnlyCollection<IEntityType>;
            _defaultShapeStyle = defaultShapeStyle;     // can be null
        }

        public string CreateNode(EntityIdentifier entityIdentifier, IReadOnlyCollection<ColumnDescriptor> columns, Action<string> onRelationship)
        {
            _ = entityIdentifier.WhenNotNull(nameof(entityIdentifier));
            _ = columns.WhenNotNullOrEmpty(nameof(columns));
            _ = onRelationship.WhenNotNull(nameof(onRelationship));

            var sb = new StringBuilder();

            var entityName = entityIdentifier.TableName;

            sb.AppendLine($"{entityName}: {{");
            sb.AppendLine("  shape: sql_table");
            sb.AppendLine();

            var entityType = _dbContextEntityTypes
                .Single(entity => entity.ClrType == entityIdentifier.Type)
                .ClrType;

            if (_options.TryGetEntityOptions(entityType, out var entityOptions))
            {
                if (!entityOptions.ShapeStyle.IsDefault())
                {
                    sb.AppendLine(entityOptions.ShapeStyle.AsText(2));
                    sb.AppendLine();
                }
            }
            else
            {
                if (_defaultShapeStyle is not null)
                {
                    sb.AppendLine(_defaultShapeStyle);
                    sb.AppendLine();
                }
            }

            foreach (var column in columns)
            {
                var columnName = column.ColumnName;
                var columnType = GetColumnDetail(entityType, column, _options);
                var columnConstraint = GetColumnConstraint(column);

                sb.AppendLine($"  {columnName}: {columnType} {columnConstraint}");

                if (column.ForeignKeyPrincipals != null)
                {
                    var relationshipNodeGenerator = new RelationshipNodeGenerator(_options);

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

        private static string GetColumnDetail(Type entityType, ColumnDescriptor column, ErdOptions configuration)
        {
            var hasEntityOptions = configuration.TryGetEntityOptions(entityType, out var options);

            var columnType = column.ColumnType;

            if (column.MaxLength.HasValue)
            {
                var showMaxLength = hasEntityOptions
                    ? options.ShowMaxLength
                    : configuration.Entities.ShowMaxLength;

                if (showMaxLength)
                {
                    columnType = $"{column.ColumnType}({column.MaxLength})";
                }
            }

            var nullableIsVisible = hasEntityOptions
                ? options.Nullable.IsVisible
                : configuration.Entities.Nullable.IsVisible;

            if (nullableIsVisible)
            {
                if (column.IsNullable && configuration.Entities.Nullable.Mode == NullableColumnMode.IsNull)
                {
                    columnType = $@"{columnType} {configuration.Entities.Nullable.IsNullLabel.D2EscapeString()}";
                }

                if (!column.IsNullable && configuration.Entities.Nullable.Mode == NullableColumnMode.NotNull)
                {
                    columnType = $@"{columnType} {configuration.Entities.Nullable.NotNullLabel.D2EscapeString()}";
                }
            }

            return columnType;
        }

        private static string GetColumnConstraint(ColumnDescriptor column)
        {
            return column.Constraint switch
            {
                ConstraintType.None => string.Empty,
                ConstraintType.PrimaryKey => $"{{ constraint: {PrimaryKey} }}",
                ConstraintType.ForeignKey => $"{{ constraint: {ForeignKey} }}",
                _ => throw new InvalidOperationException($"Unhandled constraint type '{column.Constraint}'.")
            };
        }
    }
}