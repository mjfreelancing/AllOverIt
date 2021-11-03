using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace AllOverIt.Validation
{
    /// <summary>A base validator that adds support for validating rules against custom context data.</summary>
    /// <typeparam name="TType">The model type being validated.</typeparam>
    public abstract class ValidatorBase<TType> : AbstractValidator<TType>
    {
        /// <summary>Prevents Pascal property name splitting.</summary>
        public static void DisablePropertyNameSplitting()
        {
            ValidatorOptions.Global.DisplayNameResolver = (type, info, expression) => info.Name;
        }

        /// <summary>Validates a model instance. Additional context data is associated with the request that
        /// can be utilized in the validation rules.</summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="instance">The model instance.</param>
        /// <param name="context">The context type.</param>
        /// <returns>The validation result.</returns>
        public ValidationResult Validate<TContext>(TType instance, TContext context)
        {
            var validationContext = new ValidationContext<TType>(instance);
            validationContext.SetContextData(context);

            return Validate(validationContext);
        }
    }
}