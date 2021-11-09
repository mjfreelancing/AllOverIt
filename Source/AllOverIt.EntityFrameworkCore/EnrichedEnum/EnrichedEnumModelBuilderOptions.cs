using AllOverIt.Patterns.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    /// <summary>Provides model builder options to configure entities containing properties that inherit <see cref="EnrichedEnum{TEnum}"/>.</summary>
    public sealed class EnrichedEnumModelBuilderOptions
    {
        private readonly IList<EnrichedEnumEntityOptions> _entityOptions = new List<EnrichedEnumEntityOptions>();

        // Ensures implicitly default configuration options are provided.
        internal IEnumerable<EnrichedEnumEntityOptions> EntityOptions => GetEntityOptions();

        /// <summary>Provides access to the configuration of a specified entity type.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>An options instance for the specified entity type.</returns>
        public EnrichedEnumEntityOptions Entity<TEntity>()
        {
            return Entities(typeof(TEntity));
        }

        /// <summary>Provides access to the configuration of a specified entity type.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>An options instance for the specified entity type.</returns>
        public EnrichedEnumEntityOptions Entity(Type entityType)
        {
            return Entities(entityType);
        }

        /// <summary>Provides access to the configuration of one or more entity types.</summary>
        /// <param name="entityTypes">The entity types.</param>
        /// <returns>An options instance for the specified entity types.</returns>
        public EnrichedEnumEntityOptions Entities(params Type[] entityTypes)
        {
            var entityOption = new EnrichedEnumEntityOptions(entityTypes);
            _entityOptions.Add(entityOption);

            return entityOption;
        }

        /// <summary>Configures all <see cref="EnrichedEnum{TEnum}"/> properties on all entities to store their values as a string.</summary>
        /// <param name="columnType">Optional. If provided, this configures the data type of the column that the property maps to when targeting a relational database.
        /// This should be the complete type name, including its length.</param>
        /// <param name="maxLength">Optional. If provided this value specifies the column's maximum length. This parameter is not required if the [MaxLength] attribute is used.</param>
        public void AsName(string columnType = default, int? maxLength = default)
        {
            foreach (var entityOption in GetEntityOptions())
            {
                entityOption.AsName(columnType, maxLength);
            }
        }

        /// <summary>Configures all <see cref="EnrichedEnum{TEnum}"/> properties on all entities to store their values as an integer.</summary>
        /// <param name="columnType">Optional. If provided, this configures the data type of the column that the property maps to when targeting a relational database.</param>
        public void AsValue(string columnType = default)
        {
            foreach (var entityOption in GetEntityOptions())
            {
                entityOption.AsValue(columnType);
            }
        }

        private IEnumerable<EnrichedEnumEntityOptions> GetEntityOptions()
        {
            // If nothing has been configured then add a default that will process all properties on all entities as integer values
            if (!_entityOptions.Any())
            {
                var options = new EnrichedEnumEntityOptions();
                _entityOptions.Add(options);
            }

            return _entityOptions;
        }
    }
}