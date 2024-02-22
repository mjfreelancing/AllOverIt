using AllOverIt.Assertion;
using FluentValidation;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="AbstractValidator{T}"/>.</summary>
    public static class AbstractValidatorExtensions
    {
        /// <summary>Applies a custom validation rule by combining <c>RuleFor()</c> and <c>Custom()</c>.</summary>
        /// <typeparam name="TType">The model type to validate.</typeparam>
        /// <typeparam name="TProperty">The property type to validate.</typeparam>
        /// <param name="validator">The abstract validator applying a conditional rule to.</param>
        /// <param name="propertyExpression">The expression representing the property to validate.</param>
        /// <param name="action">The custom validation rule.</param>
        public static void CustomRuleFor<TType, TProperty>(this AbstractValidator<TType> validator, Expression<Func<TType, TProperty>> propertyExpression,
            Action<TProperty, ValidationContext<TType>> action)
        {
            _ = validator.WhenNotNull(nameof(validator));
            _ = propertyExpression.WhenNotNull(nameof(propertyExpression));
            _ = action.WhenNotNull(nameof(action));

            validator
                .RuleFor(propertyExpression)
                .Custom(action);
        }

        /// <summary>Applies a custom validation rule by combining <c>RuleFor()</c> and <c>CustomAsync()</c>.</summary>
        /// <typeparam name="TType">The model type to validate.</typeparam>
        /// <typeparam name="TProperty">The property type to validate.</typeparam>
        /// <param name="validator">The abstract validator applying a conditional rule to.</param>
        /// <param name="propertyExpression">The expression representing the property to validate.</param>
        /// <param name="action">The custom validation rule.</param>
        public static void CustomRuleForAsync<TType, TProperty>(this AbstractValidator<TType> validator, Expression<Func<TType, TProperty>> propertyExpression,
            Func<TProperty, ValidationContext<TType>, CancellationToken, Task> action)
        {
            _ = validator.WhenNotNull(nameof(validator));
            _ = propertyExpression.WhenNotNull(nameof(propertyExpression));
            _ = action.WhenNotNull(nameof(action));

            validator
                .RuleFor(propertyExpression)
                .CustomAsync(action);
        }

        /// <summary>Applies a custom validation rule if a condition is met.</summary>
        /// <typeparam name="TType">The model type to validate.</typeparam>
        /// <typeparam name="TProperty">The property type to validate.</typeparam>
        /// <param name="validator">The abstract validator applying a conditional rule to.</param>
        /// <param name="propertyExpression">The expression representing the property to validate.</param>
        /// <param name="predicate">The condition to apply to a custom validation rule.</param>
        /// <param name="action">The custom validation rule.</param>
        public static void ConditionalCustomRuleFor<TType, TProperty>(this AbstractValidator<TType> validator,
            Expression<Func<TType, TProperty>> propertyExpression, Func<TType, bool> predicate,
            Action<TProperty, ValidationContext<TType>> action)
        {
            _ = validator.WhenNotNull(nameof(validator));
            _ = propertyExpression.WhenNotNull(nameof(propertyExpression));
            _ = predicate.WhenNotNull(nameof(predicate));
            _ = action.WhenNotNull(nameof(action));

            validator.When(predicate, () =>
            {
                validator.CustomRuleFor(propertyExpression, action);
            });
        }

        /// <summary>Applies a custom validation rule if a condition is met.</summary>
        /// <typeparam name="TType">The model type to validate.</typeparam>
        /// <typeparam name="TProperty">The property type to validate.</typeparam>
        /// <param name="validator">The abstract validator applying a conditional rule to.</param>
        /// <param name="propertyExpression">The expression representing the property to validate.</param>
        /// <param name="predicate">The condition to apply to a custom validation rule.</param>
        /// <param name="action">The custom validation rule.</param>
        public static void ConditionalCustomRuleForAsync<TType, TProperty>(this AbstractValidator<TType> validator,
            Expression<Func<TType, TProperty>> propertyExpression, Func<TType, bool> predicate,
            Func<TProperty, ValidationContext<TType>, CancellationToken, Task> action)
        {
            _ = validator.WhenNotNull(nameof(validator));
            _ = propertyExpression.WhenNotNull(nameof(propertyExpression));
            _ = predicate.WhenNotNull(nameof(predicate));
            _ = action.WhenNotNull(nameof(action));

            validator.When(predicate, () =>
            {
                validator.CustomRuleForAsync(propertyExpression, action);
            });
        }
    }
}