using AllOverIt.Assertion;
using AllOverIt.Patterns.Enumeration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Reflection;

namespace AllOverIt.AspNetCore.ModelBinders
{
    public class EnrichedEnumModelBinderProvider : IModelBinderProvider
    {
        private static readonly Type RichEnumType = typeof(EnrichedEnum<>);

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            _ = context.WhenNotNull(nameof(context));

            var fullyQualifiedAssemblyName = context.Metadata.ModelType.FullName;

            if (fullyQualifiedAssemblyName == null)
            {
                return null;
            }

            var enumType = context.Metadata.ModelType.Assembly.GetType(fullyQualifiedAssemblyName, false);

            var baseType = enumType?.BaseType;

            if (baseType is {IsGenericType: true} && baseType.GetGenericTypeDefinition() == RichEnumType)
            {
                var methodInfo = typeof(EnrichedEnumModelBinder).GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.Public);

                methodInfo.CheckNotNull(nameof(methodInfo));

                return methodInfo!
                    .MakeGenericMethod(enumType)
                    .Invoke(null, null) as IModelBinder;
            }

            return null;
        }
    }
}