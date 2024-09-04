using AllOverIt.EntityFrameworkCore.EnrichedEnum;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace AllOverIt.EntityFrameworkCore.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ModelBuilder"/>.</summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>Configures the model builder to store entity properties that inherit <see cref="EnrichedEnum{TENum}"/> as integer or string values.</summary>
        /// <param name="modelBuilder">The model builder being configured.</param>
        /// <param name="configure">The configuration action to invoke. If null then the model builder will be configured to store all values as integers.</param>
        public static ModelBuilder UseEnrichedEnum(this ModelBuilder modelBuilder, Action<EnrichedEnumModelBuilderOptions>? configure = default)
        {
            var options = new EnrichedEnumModelBuilderOptions();
            configure?.Invoke(options);
            UseEnrichedEnum(modelBuilder, options);

            return modelBuilder;
        }

        private static void UseEnrichedEnum(ModelBuilder modelBuilder, EnrichedEnumModelBuilderOptions options)
        {
            var allEntityTypes = modelBuilder.Model.GetEntityTypes().AsReadOnlyCollection();
            var processed = new List<(IMutableEntityType entityType, PropertyInfo propertyInfo)>();

            var entityOptions = options.EntityOptions.AsReadOnlyCollection();

            foreach (var entityOption in entityOptions)
            {
                var entityPredicate = entityOption.EntityPredicate;
                var propertyPredicate = entityOption.PropertyPredicate;
                var valueConverter = entityOption.PropertyOptions.TypeConverter;
                var propertyBuilder = entityOption.PropertyOptions.PropertyBuilder;

                var entityTypes = allEntityTypes.Where(entityPredicate);

                foreach (var entityType in entityTypes)
                {
                    var properties = entityType.ClrType.GetProperties().Where(propertyPredicate);

                    foreach (var propertyInfo in properties)
                    {
                        ConfigureEntityProperty(modelBuilder, entityType, propertyInfo, propertyBuilder, valueConverter);
                        processed.Add((entityType, propertyInfo));
                    }
                }
            }

            // Set default conversion for all entities / properties not configured
            ConfigureUnprocessed(modelBuilder, allEntityTypes, processed);
        }

        private static void ConfigureUnprocessed(ModelBuilder modelBuilder, IEnumerable<IMutableEntityType> allEntityTypes,
            List<(IMutableEntityType entityType, PropertyInfo propertyInfo)> processed)
        {
            var unprocessed =
                from entityType in allEntityTypes
                from propertyInfo in entityType.ClrType.GetProperties()
                where propertyInfo.PropertyType.IsEnrichedEnum()
                let entityProperty = (entityType, property: propertyInfo)
                where !processed.Contains(entityProperty)
                select entityProperty;

            foreach (var (entityType, propertyInfo) in unprocessed)
            {
                ConfigureEntityProperty(modelBuilder, entityType, propertyInfo, null, EnrichedEnumModelBuilderTypes.AsValueConverter);
            }
        }

        private static void ConfigureEntityProperty(ModelBuilder modelBuilder, IMutableEntityType entityType, PropertyInfo property,
            Action<PropertyBuilder>? propertyBuilder, Type valueConverter)
        {
            var converter = CreateValueConverter(property.PropertyType, valueConverter);

            var propBuilder = modelBuilder
                .Entity(entityType.Name)
                .Property(property.Name)
                .HasConversion(converter);

            propertyBuilder?.Invoke(propBuilder);
        }

        private static ValueConverter CreateValueConverter(Type propertyType, Type valueConverter)
        {
            var converterType = valueConverter.MakeGenericType(propertyType);
            return (ValueConverter) Activator.CreateInstance(converterType)!;
        }
    }
}