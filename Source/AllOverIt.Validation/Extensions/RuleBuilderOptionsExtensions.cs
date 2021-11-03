using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IRuleBuilderOptions{TType, TProperty}"/>.</summary>
    public static class RuleBuilderOptionsExtensions
    {
        /// <summary>Specifies a custom error code based on a <see cref="ValidationErrorCode"/>.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="errorCode"></param>
        public static IRuleBuilderOptions<TType, TProperty> WithErrorCode<TType, TProperty>(this IRuleBuilderOptions<TType, TProperty> ruleBuilder,
            ValidationErrorCode errorCode)
        {
            return ruleBuilder.WithErrorCode($"{errorCode}");
        }
    }
}