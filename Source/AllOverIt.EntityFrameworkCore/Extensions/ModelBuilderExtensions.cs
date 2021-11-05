using AllOverIt.EntityFrameworkCore.ValueConverters;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace AllOverIt.EntityFrameworkCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        private static readonly Type GenericEnrichedEnumType = typeof(EnrichedEnum<>);

        /// <summary>Configures an entity framework converter for all properties derived from <see cref="EnrichedEnum{TEnum}"/> and stores their Name (string) value.</summary>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="stringColumnType">The data type of the string column that the property maps to when targeting a relational database.</param>
        public static void UseEnrichedEnumName(this ModelBuilder modelBuilder, string stringColumnType)
        {
            modelBuilder.UseEnrichedEnum(typeof(EnrichedEnumNameConverter<>), stringColumnType);
        }

        /// <summary>Configures an entity framework converter for all properties derived from <see cref="EnrichedEnum{TEnum}"/> and stores their Value (integer) value.</summary>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="integerColumnType">The data type of the integer column that the property maps to when targeting a relational database.</param>
        public static void UseEnrichedEnumValue(this ModelBuilder modelBuilder, string integerColumnType)
        {
            modelBuilder.UseEnrichedEnum(typeof(EnrichedEnumValueConverter<>), integerColumnType);
        }

        private static void UseEnrichedEnum(this ModelBuilder modelBuilder, Type valueConverter, string columnType)
        {
            // IsSubclassOf() doesn't work with parameter-less generics, such as EnrichedEnum<>
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType
                    .GetProperties()
                    .Where(property => property.PropertyType.IsDerivedFrom(GenericEnrichedEnumType));

                foreach (var property in properties)
                {
                    var converterType = valueConverter.MakeGenericType(property.PropertyType);

                    var converter = (ValueConverter) Activator.CreateInstance(converterType);

                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasColumnType(columnType)
                        .HasConversion(converter);
                }
            }
        }
    }
}
