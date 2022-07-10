using AllOverIt.Aws.Cdk.AppSync.Mapping;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources
{
    /// <summary>Base class for all AppSync datasource types.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DataSourceAttribute : Attribute
    {
        /// <summary>The datasource name.</summary>
        public abstract string DataSourceName { get; }

        /// <summary>The mapping type that provides the required request and response mapping details. This type must inherit <see cref="IRequestResponseMapping"/>.</summary>
        public Type MappingType { get; }
        
        /// <summary>A description for the datasource.</summary>
        public string Description { get; }

        /// <summary>Constructor.</summary>
        /// <param name="mappingType">The mapping type, inheriting <see cref="IRequestResponseMapping"/>, that provides the datasource request and response
        /// mapping. If null, it is expected that the required mapping will be provided via <see cref="MappingTemplates"/>.</param>
        /// <param name="description">A description for the datasource.</param>
        protected DataSourceAttribute(Type mappingType, string description)
        {
            // Will be null if the mapping is being provided via code in a MappingTemplates
            if (mappingType != null)
            {
                if (!typeof(IRequestResponseMapping).IsAssignableFrom(mappingType))
                {
                    throw new InvalidOperationException($"The type '{mappingType.FullName}' must implement '{nameof(IRequestResponseMapping)}'.");
                }

                MappingType = mappingType;
            }

            Description = description;
        }
    }
}