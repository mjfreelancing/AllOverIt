using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    public sealed class EnrichedEnumModelBuilderOptions
    {
        private readonly IList<EnrichedEnumModelBuilderEntityOptions> _entityOptions = new List<EnrichedEnumModelBuilderEntityOptions>();

        public IEnumerable<EnrichedEnumModelBuilderEntityOptions> EntityOptions => GetEntityOptions();

        public EnrichedEnumModelBuilderEntityOptions Entity<TEntity>()
        {
            return Entities(typeof(TEntity));
        }

        public EnrichedEnumModelBuilderEntityOptions Entity(Type entityType)
        {
            return Entities(entityType);
        }

        public EnrichedEnumModelBuilderEntityOptions Entities(params Type[] entityTypes)
        {
            var entityOption = new EnrichedEnumModelBuilderEntityOptions(entityTypes);
            _entityOptions.Add(entityOption);

            return entityOption;
        }

        public void AsName(string columnType = default, int? maxLength = default)
        {
            foreach (var entityOption in GetEntityOptions())
            {
                entityOption.AsName(columnType, maxLength);
            }
        }

        public void AsValue(string columnType = default)
        {
            foreach (var entityOption in GetEntityOptions())
            {
                entityOption.AsValue(columnType);
            }
        }

        private IEnumerable<EnrichedEnumModelBuilderEntityOptions> GetEntityOptions()
        {
            // If nothing has been configured then add a default that will process all properties on all entities as integer values
            if (!_entityOptions.Any())
            {
                var options = new EnrichedEnumModelBuilderEntityOptions();
                _entityOptions.Add(options);
            }

            return _entityOptions;
        }
    }
}