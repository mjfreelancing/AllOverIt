using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    /// <summary>Provides options for entities containing <see cref="EnrichedEnum{TEnum}"/> properties.</summary>
    public sealed class EnrichedEnumEntityOptions
    {
        internal Func<IMutableEntityType, bool> EntityPredicate { get; }
        internal Func<PropertyInfo, bool> PropertyPredicate { get; private set; }
        internal EnrichedEnumPropertyOptions PropertyOptions { get; } = new();

        /// <summary>Constructor. The default behaviour is to include all properties on all entities that inherit <see cref="EnrichedEnum{TEnum}"/>.</summary>
        public EnrichedEnumEntityOptions()
        {
            EntityPredicate = _ => true;
            PropertyPredicate = property => property.PropertyType.IsEnrichedEnum();
        }

        /// <summary>Constructor. The property options to be configured will be applied to the specified entity types.</summary>
        /// <param name="entityTypes">One or more entity types to be configured.</param>
        public EnrichedEnumEntityOptions(IEnumerable<Type> entityTypes)
        {
            EntityPredicate = entity => entityTypes.Contains(entity.ClrType);
        }

        /// <summary>Gets options for all properties of the specified type on the entity types being configured.</summary>
        /// <typeparam name="TProperty">The property type to configure.</typeparam>
        /// <returns>An options instance for the specified property type.</returns>
        public EnrichedEnumPropertyOptions Property<TProperty>() where TProperty : EnrichedEnum<TProperty>
        {
            return Properties(typeof(TProperty));
        }

        /// <summary>Gets options for all properties of the specified type on the entity types being configured.</summary>
        /// <param name="propertyType">The property type to configure.</param>
        /// <returns>An options instance for the specified property type.</returns>
        public EnrichedEnumPropertyOptions Property(Type propertyType)
        {
            return Properties(propertyType);
        }

        /// <summary>Gets options for the specified property types on the entity types being configured.</summary>
        /// <param name="propertyTypes">One or more property types to be configured.</param>
        /// <returns>An options instance for the specified property types on the entity types being configured.</returns>
        public EnrichedEnumPropertyOptions Properties(params Type[] propertyTypes)
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

        /// <summary>Gets options for all properties of the specified name on the entity types being configured.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>An options instance for the specified property on the entity types being configured.</returns>
        public EnrichedEnumPropertyOptions Property(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return Properties(propertyName);
        }

        /// <summary>Gets options for all properties of the specified names on the entity types being configured.</summary>
        /// <param name="propertyNames">The property names.</param>
        /// <returns>An options instance for the specified properties on the entity types being configured.</returns>
        public EnrichedEnumPropertyOptions Properties(params string[] propertyNames)
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

        /// <summary>Configures all properties on the entity types being configured to store their values as a string.</summary>
        /// <param name="columnType">Optional. If provided, this configures the data type of the column that the property maps to when targeting a relational database.
        /// This should be the complete type name, including its length.</param>
        /// <param name="maxLength">Optional. If provided this value specifies the column's maximum length. This parameter is not required if the [MaxLength] attribute is used.</param>
        public void AsName(string columnType = default, int? maxLength = default)
        {
            PropertyOptions.AsName(columnType, maxLength);
        }

        /// <summary>Configures all properties on the entity types being configured to store their values as an integer.</summary>
        /// <param name="columnType">Optional. If provided, this configures the data type of the column that the property maps to when targeting a relational database.</param>
        public void AsValue(string columnType = default)
        {
            PropertyOptions.AsValue(columnType);
        }

        private static void AssertPropertyType(Type propertyType)
        {
            if (!propertyType.IsEnrichedEnum())
            {
                throw new InvalidOperationException($"The property type '{propertyType.GetFriendlyName()}' is not an EnrichedEnum.");
            }
        }
    }
}