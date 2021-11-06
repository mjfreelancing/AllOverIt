using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    public sealed class EnrichedEnumModelBuilderEntityOptions
    {
        public Func<IMutableEntityType, bool> EntityPredicate { get; }
        public Func<PropertyInfo, bool> PropertyPredicate { get; private set; }

        public EnrichedEnumModelBuilderPropertyOptions PropertyOptions { get; } = new();

        public EnrichedEnumModelBuilderEntityOptions()
        {
            EntityPredicate = _ => true;
            PropertyPredicate = property => property.PropertyType.IsDerivedFrom(EnrichedEnumModelBuilderTypes.GenericEnrichedEnumType);
        }

        public EnrichedEnumModelBuilderEntityOptions(IEnumerable<Type> entityTypes)
        {
            EntityPredicate = entity => entityTypes.Contains(entity.ClrType);
        }

        public EnrichedEnumModelBuilderPropertyOptions Property<TProperty>() where TProperty : EnrichedEnum<TProperty>
        {
            return Properties(typeof(TProperty));
        }

        public EnrichedEnumModelBuilderPropertyOptions Property(Type propertyType)
        {
            return Properties(propertyType);
        }

        public EnrichedEnumModelBuilderPropertyOptions Properties(params Type[] propertyTypes)
        {
            PropertyPredicate = property =>
            {
                if (!propertyTypes.Contains(property.PropertyType))
                {
                    return false;
                }

                AssertPropertyType(property.PropertyType);
                return true;
            };

            return PropertyOptions;
        }

        public EnrichedEnumModelBuilderPropertyOptions Property(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return Properties(propertyName);
        }

        public EnrichedEnumModelBuilderPropertyOptions Properties(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty(nameof(propertyNames));

            PropertyPredicate = property =>
            {
                if (!propertyNames.Contains(property.Name))
                {
                    return false;
                }

                AssertPropertyType(property.PropertyType);
                return true;
            };

            return PropertyOptions;
        }

        public void AsName(string columnType = default, int? maxLength = default)
        {
            PropertyOptions.AsName(columnType, maxLength);
        }

        public void AsValue(string columnType = default)
        {
            PropertyOptions.AsValue(columnType);
        }

        private static void AssertPropertyType(Type propertyType)
        {
            if (!propertyType.IsDerivedFrom(EnrichedEnumModelBuilderTypes.GenericEnrichedEnumType))
            {
                throw new InvalidOperationException(
                    $"The property type '{propertyType.GetFriendlyName()}' does not inherit '{EnrichedEnumModelBuilderTypes.GenericEnrichedEnumType.GetFriendlyName()}'.");
            }
        }
    }
}