﻿using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    /// <summary>Provides entity property options for types inheriting <see cref="EnrichedEnum{TEnum}"/> that determine how the values will be stored.</summary>
    public sealed class EnrichedEnumPropertyOptions
    {
        internal Type TypeConverter { get; private set; }
        internal Action<PropertyBuilder> PropertyBuilder { get; private set; }

        /// <summary>Constructor. The default behaviour is to store the property value as an integer.</summary>
        public EnrichedEnumPropertyOptions()
        {
            AsValue();
        }

        /// <summary>Configures the property to be stored as a string.</summary>
        /// <param name="columnType">Optional. If provided, this configures the data type of the column that the property maps to when targeting a relational database.
        /// This should be the complete type name, including its length.</param>
        /// <param name="maxLength">Optional. If provided this value specifies the column's maximum length. This parameter is not required if the [MaxLength] attribute is used.</param>
        public void AsName(string columnType = default, int? maxLength = default)
        {
            TypeConverter = EnrichedEnumModelBuilderTypes.AsNameConverter;

            var columnOptions = new EnrichedEnumStringColumnOptions
            {
                ColumnType = columnType,
                MaxLength = maxLength
            };

            PropertyBuilder = CreateStringPropertyBuilder(columnOptions);
        }

        /// <summary>Configures the property to be stored as an integer.</summary>
        /// <param name="columnType">Optional. If provided, this configures the data type of the column that the property maps to when targeting a relational database.</param>
        public void AsValue(string columnType = default)
        {
            TypeConverter = EnrichedEnumModelBuilderTypes.AsValueConverter;

            var columnOptions = new EnrichedEnumColumnOptions
            {
                ColumnType = columnType
            };

            PropertyBuilder = CreateIntegerPropertyBuilder(columnOptions);
        }

        private static Action<PropertyBuilder> CreateStringPropertyBuilder(EnrichedEnumStringColumnOptions columnOptions)
        {
            Action<PropertyBuilder> propertyBuilder = null;

            if (columnOptions != null)
            {
                if (columnOptions.ColumnType != null || columnOptions.MaxLength.HasValue)
                {
                    propertyBuilder = builder =>
                    {
                        if (columnOptions.ColumnType != null)
                        {
                            builder.HasColumnType(columnOptions.ColumnType);
                        }

                        if (columnOptions.MaxLength.HasValue)
                        {
                            builder.HasMaxLength(columnOptions.MaxLength.Value);
                        }
                    };
                }
            }

            return propertyBuilder;
        }

        private static Action<PropertyBuilder> CreateIntegerPropertyBuilder(EnrichedEnumColumnOptions columnOptions)
        {
            Action<PropertyBuilder> propertyBuilder = null;

            if (columnOptions?.ColumnType != null)
            {
                propertyBuilder = builder =>
                {
                    builder.HasColumnType(columnOptions.ColumnType);
                };
            }

            return propertyBuilder;
        }
    }
}