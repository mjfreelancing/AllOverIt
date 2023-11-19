﻿using AllOverIt.Assertion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Describes an entity column.</summary>
    public sealed class ColumnDescriptor
    {
        /// <summary>The column name.</summary>
        public string ColumnName { get; }

        /// <summary>The column type.</summary>
        public string ColumnType { get; }

        /// <summary>Indicates if the column is nullable.</summary>
        public bool IsNullable { get; }

        /// <summary>Indicates the column's maximum length, where applicable.</summary>
        public int? MaxLength { get; }

        /// <summary>Indicates the constraint type.</summary>
        public ConstraintType Constraint { get; } = ConstraintType.None;

        /// <summary>Provides foreign key principles.</summary>
        public IReadOnlyCollection<PrincipalForeignKey> ForeignKeyPrincipals { get; }

        internal ColumnDescriptor(IProperty column)
        {
            ColumnName = column.Name;
            ColumnType = column.GetColumnType();
            IsNullable = column.IsColumnNullable();

            var maxLength = column.GetAnnotations().SingleOrDefault(annotation => annotation.Name == nameof(MaxLength));

            if (maxLength is not null)
            {
                MaxLength = (int) maxLength.Value;
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

        private static List<PrincipalForeignKey> GetForeignKeys(IProperty column)
        {
            var foreignKeys = new List<PrincipalForeignKey>();

            foreach (var foreignKey in column.GetContainingForeignKeys())
            {
                var principalEntity = foreignKey.PrincipalEntityType;

                var parentToChildNavigation = foreignKey.DependentToPrincipal?.Inverse;

                Throw<InvalidOperationException>.WhenNull(
                    parentToChildNavigation,
                    $"A parent to child navigation property exists between {principalEntity.DisplayName()} and {column.DeclaringType.DisplayName()}, but not the reverse.");

                var isOneToMany = parentToChildNavigation.IsCollection;

                // TODO: Configure / handle composite keys
                var entityColumn = new PrincipalForeignKey
                {
                    EntityName = principalEntity.GetTableName(),
                    ColumnName = string.Join(", ", foreignKey.PrincipalKey.Properties.Select(property => property.Name)),
                    IsOneToMany = isOneToMany
                };

                foreignKeys.Add(entityColumn);
            }

            return foreignKeys;
        }
    }
}