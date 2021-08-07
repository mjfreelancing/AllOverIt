using AllOverIt.Validation.Validators;
using FluentValidation;
using System;

namespace AllOverIt.Validation.Extensions
{
    public static partial class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TType, TProperty> IsGreaterThanOrEqualTo<TType, TProperty>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, TProperty comparison)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(comparison)
                .WithMessage("'{PropertyName}' must be greater than or equal to {ComparisonValue}.")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        public static IRuleBuilderOptions<TType, TProperty?> IsGreaterThanOrEqualTo<TType, TProperty>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, TProperty comparison)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(comparison)
                .WithMessage("'{PropertyName}' must be greater than or equal to {ComparisonValue}.")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        public static IRuleBuilderOptions<TType, TProperty> IsGreaterThanOrEqualTo<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, Func<TContext, TProperty> contextValueResolver)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new GreaterThanOrEqualToContextValidator<TType, TProperty, TContext>(contextValueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        public static IRuleBuilderOptions<TType, TProperty?> IsGreaterThanOrEqualTo<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, Func<TContext, TProperty> contextValueResolver)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new GreaterThanOrEqualToContextValidator<TType, TProperty, TContext>(contextValueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }
    }
}