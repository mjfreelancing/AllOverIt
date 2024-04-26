using AllOverIt.Assertion;
using AllOverIt.Patterns.Enumeration;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            _ = bindingContext.WhenNotNull(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var enumerationValue = valueProviderResult.FirstValue;

            if (TryGetEnrichedEnum(enumerationValue, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();

                bindingContext.ModelState.AddModelError(bindingContext.FieldName, $"The value '{valueProviderResult.FirstValue}' is not supported.");
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