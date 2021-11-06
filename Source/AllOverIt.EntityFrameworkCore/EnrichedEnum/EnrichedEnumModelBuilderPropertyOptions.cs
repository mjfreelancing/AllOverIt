using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    public sealed class EnrichedEnumModelBuilderPropertyOptions
    {
        internal Type TypeConverter { get; private set; }
        internal Action<PropertyBuilder> PropertyBuilder { get; private set; }

        public EnrichedEnumModelBuilderPropertyOptions()
        {
            AsValue();
        }

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