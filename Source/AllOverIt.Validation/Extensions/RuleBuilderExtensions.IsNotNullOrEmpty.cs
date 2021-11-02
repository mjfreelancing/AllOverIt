using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    public static partial class RuleBuilderExtensions
    {
        /// <summary>Validation will fail if the value is null, and empty string, whitespace, an empty collection or the default value for the type.</summary>
        /// <typeparam name="TType">The type containing the property being validated.</typeparam>
        /// <typeparam name="TProperty">The property being validated.</typeparam>
        /// <param name="ruleBuilder">The rule builder being extended.</param>
        public static IRuleBuilderOptions<TType, TProperty> IsNotNullOrEmpty<TType, TProperty>(this IRuleBuilder<TType, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("'{PropertyName}' should not be empty.")
                .WithErrorCode(ValidationErrorCode.NotEmpty);
        }
    }
}