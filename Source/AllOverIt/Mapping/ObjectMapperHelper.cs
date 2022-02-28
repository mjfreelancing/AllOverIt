using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    internal static class ObjectMapperHelper
    {
        internal static IReadOnlyCollection<PropertyInfo> GetMappableProperties(Type sourceType, Type targetType, BindingOptions bindingOptions)
        {
            var sourceProps = sourceType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanRead);
            var destProps = targetType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanWrite);

            return sourceProps
                .FindMatches(
                    destProps,
                    src => $"{src.Name}.{src.PropertyType.GetFriendlyName()}",
                    target => $"{target.Name}.{target.PropertyType.GetFriendlyName()}")
                .AsReadOnlyCollection();
        }
    }
}