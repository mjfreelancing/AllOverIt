using AllOverIt.EntityFrameworkCore.ValueConverters;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Reflection;

namespace AllOverIt.EntityFrameworkCore.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ModelBuilder"/>.</summary>
    public static class ModelBuilderExtensions
    {
        private static readonly Type GenericEnrichedEnumType = typeof(EnrichedEnum<>);

        /// <summary>Configures the model builder so all properties derived from <see cref="EnrichedEnum{T}"/> across all entity types
        /// will store the enumeration's Name (string) in a string column.</summary>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the string column that the property maps
        /// to when targeting a relational database. This must be the complete type name, including length, applicable for the database in use.</param>
        /// <param name="maxLength">Optional. If provided this value specifies the column's maximum length. This parameter is not required
        /// if the [MaxLength] attribute is used.</param>
        public static void UseEnrichedEnumName(this ModelBuilder modelBuilder, string columnType = default, int? maxLength = default)
        {
            var propertyBuilder = CreateStringPropertyBuilder(columnType, maxLength);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumNameConverter<>),
                _ => true,
                property => property.PropertyType.IsDerivedFrom(GenericEnrichedEnumType),
                propertyBuilder);
        }

        /// <summary>Configures the model builder so all properties of type <typeparam name="TProperty"/> across all entity types
        /// will store the enumeration's Name (string) in a string column. The property type must inherit <see cref="EnrichedEnum{T}"/>.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the string column that the property maps
        /// to when targeting a relational database. This must be the complete type name, including length, applicable for the database in use.</param>
        /// <param name="maxLength">Optional. If provided this value specifies the column's maximum length. This parameter is not required
        /// if the [MaxLength] attribute is used.</param>
        public static void UseEnrichedEnumName<TProperty>(this ModelBuilder modelBuilder, string columnType = default, int? maxLength = default)
            where TProperty : EnrichedEnum<TProperty>
        {
            var propertyBuilder = CreateStringPropertyBuilder(columnType, maxLength);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumNameConverter<>),
                _ => true,
                property => property.PropertyType == typeof(TProperty),
                propertyBuilder);
        }

        /// <summary>Configures the model builder so the <typeparam name="TEntity"/> entity type and all properties of type
        /// <typeparam name="TProperty"/> will store the enumeration's Name (string) in a string column. The property type must inherit
        /// <see cref="EnrichedEnum{T}"/>.</summary>
        /// <typeparam name="TEntity">The entity type to match.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the string column that the property maps
        /// to when targeting a relational database. This must be the complete type name, including length, applicable for the database in use.</param>
        /// <param name="maxLength">Optional. If provided this value specifies the column's maximum length. This parameter is not required
        /// if the [MaxLength] attribute is used.</param>
        public static void UseEnrichedEnumName<TEntity, TProperty>(this ModelBuilder modelBuilder, string columnType = default, int? maxLength = default)
            where TProperty : EnrichedEnum<TProperty>
        {
            var propertyBuilder = CreateStringPropertyBuilder(columnType, maxLength);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumNameConverter<>),
                entity => entity.ClrType == typeof(TEntity),
                property => property.PropertyType == typeof(TProperty),
                propertyBuilder);
        }

        /// <summary>Configures the model builder so the <typeparam name="TEntity"/> entity type's <param name="propertyName"/> property
        /// will store the enumeration's Name (string) in a string column. The property type must inherit <see cref="EnrichedEnum{T}"/>.</summary>
        /// <typeparam name="TEntity">The entity type to match.</typeparam>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="propertyName">The property name to match.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the string column that the property maps
        /// to when targeting a relational database. This must be the complete type name, including length, applicable for the database in use.</param>
        /// <param name="maxLength">Optional. If provided this value specifies the column's maximum length. This parameter is not required
        /// if the [MaxLength] attribute is used.</param>
        public static void UseEnrichedEnumName<TEntity>(this ModelBuilder modelBuilder, string propertyName, string columnType = default, int? maxLength = default)
        {
            var propertyBuilder = CreateStringPropertyBuilder(columnType, maxLength);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumNameConverter<>),
                entity => entity.ClrType == typeof(TEntity),
                property => property.PropertyType.IsDerivedFrom(GenericEnrichedEnumType) && property.Name == propertyName,
                propertyBuilder);
        }

        /// <summary>Configures the model builder so all properties derived from <see cref="EnrichedEnum{T}"/> across all entity types
        /// will store the enumeration's Value (integer) in an integer column</summary>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the integer column that the property maps
        /// to when targeting a relational database.</param>
        public static void UseEnrichedEnumValue(this ModelBuilder modelBuilder, string columnType = default)
        {
            var propertyBuilder = CreateIntegerPropertyBuilder(columnType);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumValueConverter<>),
                _ => true,
                _ => true,
                propertyBuilder);
        }

        /// <summary>Configures the model builder so all properties of type <typeparam name="TProperty"/> across all entity types
        /// will store the enumeration's Value (integer) in an integer column. The property type must inherit <see cref="EnrichedEnum{T}"/>.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the integer column that the property maps
        /// to when targeting a relational database.</param>
        public static void UseEnrichedEnumValue<TProperty>(this ModelBuilder modelBuilder, string columnType = default)
            where TProperty : EnrichedEnum<TProperty>
        {
            var propertyBuilder = CreateIntegerPropertyBuilder(columnType);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumValueConverter<>),
                _ => true,
                property => property.PropertyType == typeof(TProperty),
                propertyBuilder);
        }

        /// <summary>Configures the model builder so the <typeparam name="TEntity"/> entity type and all properties of type
        /// <typeparam name="TProperty"/> will store the enumeration's Value (integer) in an integer column. The property type must inherit
        /// <see cref="EnrichedEnum{T}"/>.</summary>
        /// <typeparam name="TEntity">The entity type to match.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the integer column that the property maps
        /// to when targeting a relational database.</param>
        public static void UseEnrichedEnumValue<TEntity, TProperty>(this ModelBuilder modelBuilder, string propertyName = default,
            string columnType = default)
            where TProperty : EnrichedEnum<TProperty>
        {
            var propertyBuilder = CreateIntegerPropertyBuilder(columnType);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumValueConverter<>),
                entity => entity.ClrType == typeof(TEntity),
                property => property.PropertyType == typeof(TProperty) && (propertyName == null || property.Name == propertyName),
                propertyBuilder);
        }

        /// <summary>Configures the model builder so the <typeparam name="TEntity"/> entity type's <param name="propertyName"/> property
        /// will store the enumeration's Value (integer) in an integer column. The property type must inherit <see cref="EnrichedEnum{T}"/>.</summary>
        /// <typeparam name="TEntity">The entity type to match.</typeparam>
        /// <param name="modelBuilder">The model builder instance.</param>
        /// <param name="propertyName">The property name to match.</param>
        /// <param name="columnType">Optional. If provided this value is used as the data type of the integer column that the property maps
        /// to when targeting a relational database.</param>
        public static void UseEnrichedEnumValue<TEntity>(this ModelBuilder modelBuilder, string propertyName, string columnType = default)
        {
            var propertyBuilder = CreateIntegerPropertyBuilder(columnType);

            modelBuilder.UseEnrichedEnum(
                typeof(EnrichedEnumValueConverter<>),
                entity => entity.ClrType == typeof(TEntity),
                property => property.PropertyType.IsDerivedFrom(GenericEnrichedEnumType) && property.Name == propertyName,
                propertyBuilder);
        }

        private static void UseEnrichedEnum(this ModelBuilder modelBuilder, Type valueConverter, Func<IMutableEntityType, bool> entityPredicate,
            Func<PropertyInfo, bool> propertyPredicate, Action<PropertyBuilder> propertyBuilder)
        {
            var entities = modelBuilder.Model.GetEntityTypes().Where(entityPredicate);

            foreach (var entityType in entities)
            {
                var properties = entityType.ClrType.GetProperties().Where(propertyPredicate);

                foreach (var property in properties)
                {
                    var converterType = valueConverter.MakeGenericType(property.PropertyType);

                    var converter = (ValueConverter) Activator.CreateInstance(converterType);

                    var propBuilder = modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(converter);

                    propertyBuilder?.Invoke(propBuilder);
                }
            }
        }

        private static Action<PropertyBuilder> CreateStringPropertyBuilder(string columnType, int? maxLength)
        {
            Action<PropertyBuilder> propertyBuilder = null;

            if (columnType != null || maxLength.HasValue)
            {
                propertyBuilder = builder =>
                {
                    if (columnType != null)
                    {
                        builder.HasColumnType(columnType);
                    }

                    if (maxLength.HasValue)
                    {
                        builder.HasMaxLength(maxLength.Value);
                    }
                };
            }

            return propertyBuilder;
        }

        private static Action<PropertyBuilder> CreateIntegerPropertyBuilder(string columnType)
        {
            Action<PropertyBuilder> propertyBuilder = null;

            if (columnType != null)
            {
                propertyBuilder = builder =>
                {
                    builder.HasColumnType(columnType);
                };
            }

            return propertyBuilder;
        }
    }
}
