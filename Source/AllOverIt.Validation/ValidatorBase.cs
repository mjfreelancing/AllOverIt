using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace AllOverIt.Validation
{
    // Adds support for validating rules against a context
    public abstract class ValidatorBase<TType> : AbstractValidator<TType>
    {
        public static void DisablePropertyNameSplitting()
        {
            // prevent Pascal name splitting
            ValidatorOptions.Global.DisplayNameResolver = (type, info, expression) => info.Name;
        }

        // The context contains data that can be used by a rule at the time of its invocation
        public ValidationResult Validate<TContext>(TType instance, TContext context)
        {
            var validationContext = new ValidationContext<TType>(instance);
            validationContext.SetContextData(context);

            return Validate(validationContext);
        }
    }

    // TODO - Consider how to validate based on more than one type

}