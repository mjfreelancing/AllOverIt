using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Helpers;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequestResponseMappingAttribute : Attribute
    {
        public IRequestResponseMapping MappingType { get; }

        public RequestResponseMappingAttribute(Type mappingType)
        {
            _ = mappingType.WhenNotNull(nameof(mappingType));

            if (!typeof(IRequestResponseMapping).IsAssignableFrom(mappingType))
            {
                throw new InvalidOperationException($"The type '{mappingType.FullName}' must implement '{nameof(IRequestResponseMapping)}'");
            }

            MappingType = (IRequestResponseMapping) Activator.CreateInstance(mappingType);
        }
    }
}