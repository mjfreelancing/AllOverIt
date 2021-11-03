using AllOverIt.Validation.Validators;
using FluentValidation;
using System;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IRuleBuilder{TType, TProperty}"/>.</summary>
    public static partial class RuleBuilderExtensions
    {
        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is outside the exclusive range.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="from">The lower limit of the exclusive range.</param>
        /// <param name="to">The higher limit of the exclusive range.</param>
        public static IRuleBuilderOptions<TType, TProperty> IsExclusiveBetween<TType, TProperty>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, TProperty from, TProperty to)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .ExclusiveBetween(from, to)
                .WithMessage("'{PropertyName}' must be between {From} and {To} (exclusive).")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is outside the specified range.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="from">The lower limit of the exclusive range.</param>
        /// <param name="to">The higher limit of the exclusive range.</param>
        public static IRuleBuilderOptions<TType, TProperty?> IsExclusiveBetween<TType, TProperty>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, TProperty from, TProperty to)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .ExclusiveBetween(from, to)
                .WithMessage("'{PropertyName}' must be between {From} and {To} (exclusive).")
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is outside the exclusive range provided
        /// by resolvers that have access to the root context data.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContext">The root context type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="fromValueResolver">The resolver that provides the lower exclusive limit.</param>
        /// <param name="toValueResolver">The resolver that provides the upper exclusive limit.</param>
        public static IRuleBuilderOptions<TType, TProperty> IsExclusiveBetween<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty> ruleBuilder, Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new ExclusiveBetweenContextValidator<TType, TProperty, TContext>(fromValueResolver, toValueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }

        /// <summary>Defines a validator on the current rule builder that will fail if the value of the property is outside the exclusive range provided
        /// by resolvers that have access to the root context data.</summary>
        /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TContext">The root context type.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="fromValueResolver">The resolver that provides the lower exclusive limit.</param>
        /// <param name="toValueResolver">The resolver that provides the upper exclusive limit.</param>
        public static IRuleBuilderOptions<TType, TProperty?> IsExclusiveBetween<TType, TProperty, TContext>(
            this IRuleBuilder<TType, TProperty?> ruleBuilder, Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
            where TProperty : struct, IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new ExclusiveBetweenContextValidator<TType, TProperty, TContext>(fromValueResolver, toValueResolver))
                .WithErrorCode(ValidationErrorCode.OutOfRange);
        }
    }
}