using AllOverIt.Validation.Validators;
using FluentValidation;
using System;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IRuleBuilder{TType, TProperty}"/>.</summary>
    public static partial class RuleBuilderExtensions
    {
        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is less
        /// than a specified value. The error code is set to <see cref="ValidationErrorCode.OutOfRange"/>.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="comparison">The value to compare the property value with.</param>
        public static IRuleBuilderOptions<TType, TProperty> IsLessThan<TType, TProperty>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, TProperty comparison)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .LessThan(comparison)
                .WithMessage("'{PropertyName}' must be less than {ComparisonValue}.")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is less
        /// than a specified value. The error code is set to <see cref="ValidationErrorCode.OutOfRange"/>.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="comparison">The value to compare the property value with.</param>
        public static IRuleBuilderOptions<TType, TProperty?> IsLessThan<TType, TProperty>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, TProperty comparison)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .LessThan(comparison)
                .WithMessage("'{PropertyName}' must be less than {ComparisonValue}.")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is less
        /// than the value provided by a resolver that has access to the root context data.
        /// The error code is set to <see cref="ValidationErrorCode.OutOfRange"/>.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContext">The root context type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="valueResolver">The resolver that provides the value to be compared with.</param>
        public static IRuleBuilderOptions<TType, TProperty> IsLessThan<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, Func<TContext, TProperty> valueResolver)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new LessThanContextValidator<TType, TProperty, TContext>(valueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is less
        /// than the value provided by a resolver that has access to the root context data.
        /// The error code is set to <see cref="ValidationErrorCode.OutOfRange"/>.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContext">The root context type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="valueResolver">The resolver that provides the value to be compared with.</param>
        public static IRuleBuilderOptions<TType, TProperty?> IsLessThan<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, Func<TContext, TProperty> valueResolver)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new LessThanContextValidator<TType, TProperty, TContext>(valueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }
    }
}