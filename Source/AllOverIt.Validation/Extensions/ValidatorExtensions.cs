using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    public static class ValidatorExtensions
    {
        public static void ValidateAndThrow<TType, TContext>(this IValidator<TType> validator, TType instance,
            TContext context)
        {
            var validationContext = ValidationContext<TType>.CreateWithOptions(instance, options => options.ThrowOnFailures());
            validationContext.SetContextData(context);

            validator.Validate(validationContext);
        }
    }
}