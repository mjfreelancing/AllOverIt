using AllOverIt.Patterns.Enumeration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace AllOverIt.AspNetCore.ModelBinders
{
    /// <summary>A factory for creating an <see cref="EnrichedEnum{TEnum}"/> specific model binder.</summary>
    public static class EnrichedEnumModelBinder
    {
        /// <summary>Creates an <see cref="EnrichedEnum{TEnum}"/> specific model binder.</summary>
        public static EnrichedEnumModelBinder<TEnum> CreateInstance<TEnum>()
            where TEnum : EnrichedEnum<TEnum>
        {
            return new EnrichedEnumModelBinder<TEnum>();
        }
    }

    /// <summary>Implements an <see cref="EnrichedEnum{TEnum}"/> specific model binder.</summary>
    /// <typeparam name="TEnum">The concrete <see cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public sealed class EnrichedEnumModelBinder<TEnum> : IModelBinder
        where TEnum : EnrichedEnum<TEnum>
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var enumerationName = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);

            var enumerationValue = enumerationName.FirstValue;

            TEnum result = null;

            // Removing quotes
            if (enumerationValue == null || TryGetEnrichedEnum(enumerationName.FirstValue, out result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();

                bindingContext.ModelState.AddModelError(bindingContext.FieldName, $"The value '{enumerationName.FirstValue}' is not supported.");
            }

            return Task.CompletedTask;
        }

        private static bool TryGetEnrichedEnum(string value, out TEnum result)
        {
            // Removing leading/trailing quotes in case they are provided on a query string
            value = value.Trim('"');
            return EnrichedEnum<TEnum>.TryFromNameOrValue(value, out result);
        }
    }
}