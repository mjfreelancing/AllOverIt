using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IValidator{TType}"/>.</summary>
    public static class ValidatorExtensions
    {
        /// <summary>Adds custom context data to the validation request and validates the specified model instance. If the validation
        /// fails then an exception is thrown.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TContext">The context data type.</typeparam>
        /// <param name="validator">The validator instance.</param>
        /// <param name="instance">The model instance to validate.</param>
        /// <param name="context">The context to associate with the validation request.</param>
        public static void ValidateAndThrow<TType, TContext>(this IValidator<TType> validator, TType instance, TContext context)
        {
            var validationContext = ValidationContext<TType>.CreateWithOptions(instance, options => options.ThrowOnFailures());
            validationContext.SetContextData(context);

            validator.Validate(validationContext);
        }

        /// <summary>Adds custom context data to the validation request and validates the specified model instance. If the validation
        /// fails then an exception is thrown.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TContext">The context data type.</typeparam>
        /// <param name="validator">The validator instance.</param>
        /// <param name="instance">The model instance to validate.</param>
        /// <param name="context">The context to associate with the validation request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public static Task ValidateAndThrowAsync<TType, TContext>(this IValidator<TType> validator, TType instance, TContext context, CancellationToken cancellationToken)
        {
            var validationContext = ValidationContext<TType>.CreateWithOptions(instance, options => options.ThrowOnFailures());
            validationContext.SetContextData(context);

            return validator.ValidateAsync(validationContext, cancellationToken);
        }
    }
}