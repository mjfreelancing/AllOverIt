using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace AllOverIt.AspNetCore.ModelBinders
{
    /// <summary>A model binder provider for all <see cref="EnrichedEnum{T}"/> types.</summary>
    public sealed class EnrichedEnumModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            _ = context.WhenNotNull(nameof(context));

            var fullyQualifiedName = context.Metadata.ModelType.FullName;

            if (fullyQualifiedName == null)
            {
                return null;
            }

            var enumType = context.Metadata.ModelType.Assembly.GetType(fullyQualifiedName, false);

            if (enumType.IsEnrichedEnum())
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