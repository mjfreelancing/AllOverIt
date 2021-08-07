using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    public static partial class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TType, TProperty> IsNotNullOrEmpty<TType, TProperty>(this IRuleBuilder<TType, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("'{PropertyName}' should not be empty.")
                .WithErrorCode(ValidationErrorCode.NotEmpty);
        }
    }
}