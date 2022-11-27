using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IRuleBuilder{TType, TProperty}"/>.</summary>
    public static partial class RuleBuilderExtensions
    {
        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is null, an empty string, whitespace,
        /// an empty collection or the default value of the type. The error code is set to <see cref="ValidationErrorCode.Required"/>.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        public static IRuleBuilderOptions<TType, TProperty> IsRequired<TType, TProperty>(this IRuleBuilder<TType, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("'{PropertyName}' requires a valid value.")
                .WithErrorCode(ValidationErrorCode.Required);
        }
    }
}