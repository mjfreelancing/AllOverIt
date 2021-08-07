using AllOverIt.Validation.Validators;
using FluentValidation;
using System;

namespace AllOverIt.Validation.Extensions
{
    public static partial class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TType, TProperty> IsInclusiveBetween<TType, TProperty>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, TProperty from, TProperty to)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .InclusiveBetween(from, to)
                .WithMessage("'{PropertyName}' must be between {From} and {To} (inclusive).")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        public static IRuleBuilderOptions<TType, TProperty?> IsInclusiveBetween<TType, TProperty>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, TProperty from, TProperty to)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .InclusiveBetween(from, to)
                .WithMessage("'{PropertyName}' must be between {From} and {To} (inclusive).")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        public static IRuleBuilderOptions<TType, TProperty> IsInclusiveBetween<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new InclusiveBetweenContextValidator<TType, TProperty, TContext>(fromValueResolver, toValueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        public static IRuleBuilderOptions<TType, TProperty?> IsInclusiveBetween<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new InclusiveBetweenContextValidator<TType, TProperty, TContext>(fromValueResolver, toValueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }
    }
}