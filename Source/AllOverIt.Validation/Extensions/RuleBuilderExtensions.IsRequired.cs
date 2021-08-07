using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    public static partial class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TType, TProperty> IsRequired<TType, TProperty>(this IRuleBuilder<TType, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("'{PropertyName}' requires a valid value.")
                .WithErrorCode(ValidationErrorCode.Required);
        }
    }
}