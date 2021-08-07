using AllOverIt.Validation.Extensions;
using FluentValidation;

namespace AllOverIt.Validation.Extensions
{
    public static class RuleBuilderOptionsExtensions
    {
        public static IRuleBuilderOptions<TType, TProperty> WithErrorCode<TType, TProperty>(this IRuleBuilderOptions<TType, TProperty> ruleBuilder,
            ValidationErrorCode errorCode)
        {
            return ruleBuilder.WithErrorCode($"{errorCode}");
        }
    }
}