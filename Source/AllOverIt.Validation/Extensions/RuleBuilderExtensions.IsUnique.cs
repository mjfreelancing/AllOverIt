using AllOverIt.Validation.Validators;
using FluentValidation;
using System.Linq.Expressions;

namespace AllOverIt.Validation.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IRuleBuilder{TType, TProperty}"/>.</summary>
    public static partial class RuleBuilderExtensions
    {
        /// <summary>
        /// Adds a validator to ensure that the elements in the collection are unique based on a specified property.
        /// </summary>
        /// <typeparam name="TType">The type of the object being validated.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TProperty1">The type of the property to check for uniqueness.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="propertySelector">An expression to select the property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        /// <returns>The rule builder options.</returns>
        public static IRuleBuilderOptions<TType, IEnumerable<TElement?>> IsUnique<TType, TElement, TProperty1>(
            this IRuleBuilder<TType, IEnumerable<TElement>> ruleBuilder, Expression<Func<TElement, TProperty1>> propertySelector,
            IEqualityComparer<TProperty1>? comparer = default)
        {
            return ruleBuilder.SetValidator(new IsUniqueValidator<TElement, TProperty1>(propertySelector, comparer));
        }

        /// <summary>
        /// Adds a validator to ensure that the elements in the collection are unique based on two specified properties.
        /// </summary>
        /// <typeparam name="TType">The type of the object being validated.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TProperty1">The type of the first property to check for uniqueness.</typeparam>
        /// <typeparam name="TProperty2">The type of the second property to check for uniqueness.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="property1Selector">An expression to select the first property to check for uniqueness.</param>
        /// <param name="property2Selector">An expression to select the second property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        /// <returns>The rule builder options.</returns>
        public static IRuleBuilderOptions<TType, IEnumerable<TElement?>> IsUnique<TType, TElement, TProperty1, TProperty2>(
            this IRuleBuilder<TType, IEnumerable<TElement>> ruleBuilder, Expression<Func<TElement, TProperty1>> property1Selector,
            Expression<Func<TElement, TProperty2>> property2Selector, IEqualityComparer<(TProperty1, TProperty2)>? comparer = default)
        {
            return ruleBuilder.SetValidator(new IsUniqueValidator<TElement, TProperty1, TProperty2>(property1Selector, property2Selector, comparer));
        }

        /// <summary>
        /// Adds a validator to ensure that the elements in the collection are unique based on three specified properties.
        /// </summary>
        /// <typeparam name="TType">The type of the object being validated.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TProperty1">The type of the first property to check for uniqueness.</typeparam>
        /// <typeparam name="TProperty2">The type of the second property to check for uniqueness.</typeparam>
        /// <typeparam name="TProperty3">The type of the third property to check for uniqueness.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="property1Selector">An expression to select the first property to check for uniqueness.</param>
        /// <param name="property2Selector">An expression to select the second property to check for uniqueness.</param>
        /// <param name="property3Selector">An expression to select the third property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        /// <returns>The rule builder options.</returns>
        public static IRuleBuilderOptions<TType, IEnumerable<TElement?>> IsUnique<TType, TElement, TProperty1, TProperty2, TProperty3>(
            this IRuleBuilder<TType, IEnumerable<TElement>> ruleBuilder, Expression<Func<TElement, TProperty1>> property1Selector,
            Expression<Func<TElement, TProperty2>> property2Selector, Expression<Func<TElement, TProperty3>> property3Selector,
            IEqualityComparer<(TProperty1, TProperty2, TProperty3)>? comparer = default)
        {
            return ruleBuilder.SetValidator(new IsUniqueValidator<TElement, TProperty1, TProperty2, TProperty3>(property1Selector, property2Selector, property3Selector, comparer));
        }

        /// <summary>
        /// Adds a validator to ensure that the elements in the collection are unique based on four specified properties.
        /// </summary>
        /// <typeparam name="TType">The type of the object being validated.</typeparam>
        /// <typeparam name="TElement">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TProperty1">The type of the first property to check for uniqueness.</typeparam>
        /// <typeparam name="TProperty2">The type of the second property to check for uniqueness.</typeparam>
        /// <typeparam name="TProperty3">The type of the third property to check for uniqueness.</typeparam>
        /// <typeparam name="TProperty4">The type of the fourth property to check for uniqueness.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="property1Selector">An expression to select the first property to check for uniqueness.</param>
        /// <param name="property2Selector">An expression to select the second property to check for uniqueness.</param>
        /// <param name="property3Selector">An expression to select the third property to check for uniqueness.</param>
        /// <param name="property4Selector">An expression to select the fourth property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        /// <returns>The rule builder options.</returns>
        public static IRuleBuilderOptions<TType, IEnumerable<TElement?>> IsUnique<TType, TElement, TProperty1, TProperty2, TProperty3, TProperty4>(
            this IRuleBuilder<TType, IEnumerable<TElement>> ruleBuilder, Expression<Func<TElement, TProperty1>> property1Selector,
            Expression<Func<TElement, TProperty2>> property2Selector, Expression<Func<TElement, TProperty3>> property3Selector,
            Expression<Func<TElement, TProperty4>> property4Selector, IEqualityComparer<(TProperty1, TProperty2, TProperty3, TProperty4)>? comparer = default)
        {
            return ruleBuilder.SetValidator(new IsUniqueValidator<TElement, TProperty1, TProperty2, TProperty3, TProperty4>(property1Selector, property2Selector, property3Selector, property4Selector, comparer));
        }
    }
}