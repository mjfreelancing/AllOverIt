using AllOverIt.Patterns.Enumeration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace AllOverIt.AspNetCore.ModelBinders
{
    public static class EnrichedEnumModelBinder
    {
        public static EnrichedEnumModelBinder<TEnum> CreateInstance<TEnum>()
            where TEnum : EnrichedEnum<TEnum>
        {
            return new EnrichedEnumModelBinder<TEnum>();
        }
    }

    public class EnrichedEnumModelBinder<TEnum> : IModelBinder
        where TEnum : EnrichedEnum<TEnum>
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var enumerationName = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);

            var enumerationValue = enumerationName.FirstValue;

            TEnum result = null;

            if (enumerationValue == null || EnrichedEnum<TEnum>.TryFromNameOrValue(enumerationName.FirstValue, out result))
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
    }
}