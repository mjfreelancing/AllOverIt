using AllOverIt.Aws.Cdk.AppSync.Mapping;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DataSourceAttribute : Attribute
    {
        public abstract string DataSourceName { get; }
        public Type MappingType { get; }
        public string Description { get; }

        protected DataSourceAttribute(Type mappingType, string description)
        {
            // Will be null if the mapping is being provided via code in a MappingTemplates
            if (mappingType != null)
            {
                if (!typeof(IRequestResponseMapping).IsAssignableFrom(mappingType))
                {
                    throw new InvalidOperationException($"The type '{mappingType.FullName}' must implement '{nameof(IRequestResponseMapping)}'");
                }

                MappingType = mappingType;
            }

            Description = description;
        }
    }
}