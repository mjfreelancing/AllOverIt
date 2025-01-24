using AllOverIt.Extensions;
using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;

namespace AllOverIt.Validation.Validators
{
    /// <summary>
    /// Validator to ensure that the elements in the collection are unique based on a specified property.
    /// </summary>
    /// <typeparam name="TType">The type of the elements in the collection.</typeparam>
    /// <typeparam name="TProperty">The type of the property to check for uniqueness.</typeparam>
    public sealed class IsUniqueValidator<TType, TProperty> : AbstractValidator<IEnumerable<TType>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsUniqueValidator{TType, TProperty}"/> class.
        /// </summary>
        /// <param name="propertySelector">An expression to select the property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        public IsUniqueValidator(Expression<Func<TType, TProperty?>> propertySelector, IEqualityComparer<TProperty?>? comparer = default)
        {
            RuleFor(collection => collection)
                .Custom((collection, context) =>
                {
                    if (collection is null)
                    {
                        return;
                    }

                    var (propertyName, func) = IsUnique.GetPropertyNameAndFunc(propertySelector);

                    var duplicateValues = IsUnique.GetDuplicates(collection, func, comparer);

                    if (duplicateValues.Length != 0)
                    {
                        var failure = new ValidationFailure(
                            propertyName: propertyName,
                            errorMessage: $"'{propertyName}' must have unique values.",
                            attemptedValue: duplicateValues
                        )
                        {
                            ErrorCode = nameof(ValidationErrorCode.Duplicate)
                        };

                        context.AddFailure(failure);
                    }
                });
        }
    }

    /// <summary>
    /// Validator to ensure that the elements in the collection are unique based on two specified properties.
    /// </summary>
    /// <typeparam name="TType">The type of the elements in the collection.</typeparam>
    /// <typeparam name="TProperty1">The type of the first property to check for uniqueness.</typeparam>
    /// <typeparam name="TProperty2">The type of the second property to check for uniqueness.</typeparam>
    public sealed class IsUniqueValidator<TType, TProperty1, TProperty2> : AbstractValidator<IEnumerable<TType>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsUniqueValidator{TType, TProperty1, TProperty2}"/> class.
        /// </summary>
        /// <param name="property1Selector">An expression to select the first property to check for uniqueness.</param>
        /// <param name="property2Selector">An expression to select the second property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        public IsUniqueValidator(Expression<Func<TType, TProperty1?>> property1Selector, Expression<Func<TType, TProperty2?>> property2Selector,
            IEqualityComparer<(TProperty1?, TProperty2?)>? comparer = default)
        {
            RuleFor(collection => collection)
                .Custom((collection, context) =>
                {
                    if (collection is null)
                    {
                        return;
                    }

                    var (property1Name, func1) = IsUnique.GetPropertyNameAndFunc(property1Selector);
                    var (property2Name, func2) = IsUnique.GetPropertyNameAndFunc(property2Selector);

                    var duplicateValues = IsUnique.GetDuplicates(collection, item => (func1.Invoke(item), func2.Invoke(item)), comparer);

                    if (duplicateValues.Length != 0)
                    {
                        var values = duplicateValues.SelectToArray(value => $"{IsUnique.AsString(value.Item1)}, {IsUnique.AsString(value.Item2)}");

                        var failure = new ValidationFailure(
                            propertyName: $"{property1Name}, {property2Name}",
                            errorMessage: $"The combination of '{property1Name}' and '{property2Name}' must be unique.",
                            attemptedValue: values
                        )
                        {
                            ErrorCode = nameof(ValidationErrorCode.Duplicate)
                        };

                        context.AddFailure(failure);
                    }
                });
        }
    }

    /// <summary>
    /// Validator to ensure that the elements in the collection are unique based on three specified properties.
    /// </summary>
    /// <typeparam name="TType">The type of the elements in the collection.</typeparam>
    /// <typeparam name="TProperty1">The type of the first property to check for uniqueness.</typeparam>
    /// <typeparam name="TProperty2">The type of the second property to check for uniqueness.</typeparam>
    /// <typeparam name="TProperty3">The type of the third property to check for uniqueness.</typeparam>
    public sealed class IsUniqueValidator<TType, TProperty1, TProperty2, TProperty3> : AbstractValidator<IEnumerable<TType>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsUniqueValidator{TType, TProperty1, TProperty2, TProperty3}"/> class.
        /// </summary>
        /// <param name="property1Selector">An expression to select the first property to check for uniqueness.</param>
        /// <param name="property2Selector">An expression to select the second property to check for uniqueness.</param>
        /// <param name="property3Selector">An expression to select the third property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        public IsUniqueValidator(Expression<Func<TType, TProperty1?>> property1Selector, Expression<Func<TType, TProperty2?>> property2Selector,
            Expression<Func<TType, TProperty3?>> property3Selector, IEqualityComparer<(TProperty1?, TProperty2?, TProperty3?)>? comparer = default)
        {
            RuleFor(collection => collection)
                .Custom((collection, context) =>
                {
                    if (collection is null)
                    {
                        return;
                    }

                    var (property1Name, func1) = IsUnique.GetPropertyNameAndFunc(property1Selector);
                    var (property2Name, func2) = IsUnique.GetPropertyNameAndFunc(property2Selector);
                    var (property3Name, func3) = IsUnique.GetPropertyNameAndFunc(property3Selector);

                    var duplicateValues = IsUnique.GetDuplicates(collection, item => (func1.Invoke(item), func2.Invoke(item), func3.Invoke(item)), comparer);

                    if (duplicateValues.Length != 0)
                    {
                        var values = duplicateValues.SelectToArray(value => $"{IsUnique.AsString(value.Item1)}, {IsUnique.AsString(value.Item2)}, {IsUnique.AsString(value.Item3)}");

                        var failure = new ValidationFailure(
                            propertyName: $"{property1Name}, {property2Name}, {property3Name}",
                            errorMessage: $"The combination of '{property1Name}', '{property2Name}' and '{property3Name}' must be unique.",
                            attemptedValue: values
                        )
                        {
                            ErrorCode = nameof(ValidationErrorCode.Duplicate)
                        };

                        context.AddFailure(failure);
                    }
                });
        }
    }

    /// <summary>
    /// Validator to ensure that the elements in the collection are unique based on four specified properties.
    /// </summary>
    /// <typeparam name="TType">The type of the elements in the collection.</typeparam>
    /// <typeparam name="TProperty1">The type of the first property to check for uniqueness.</typeparam>
    /// <typeparam name="TProperty2">The type of the second property to check for uniqueness.</typeparam>
    /// <typeparam name="TProperty3">The type of the third property to check for uniqueness.</typeparam>
    /// <typeparam name="TProperty4">The type of the fourth property to check for uniqueness.</typeparam>
    public sealed class IsUniqueValidator<TType, TProperty1, TProperty2, TProperty3, TProperty4> : AbstractValidator<IEnumerable<TType>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsUniqueValidator{TType, TProperty1, TProperty2, TProperty3, TProperty4}"/> class.
        /// </summary>
        /// <param name="property1Selector">An expression to select the first property to check for uniqueness.</param>
        /// <param name="property2Selector">An expression to select the second property to check for uniqueness.</param>
        /// <param name="property3Selector">An expression to select the third property to check for uniqueness.</param>
        /// <param name="property4Selector">An expression to select the fourth property to check for uniqueness.</param>
        /// <param name="comparer">An optional comparer to use for comparing property values.</param>
        public IsUniqueValidator(Expression<Func<TType, TProperty1?>> property1Selector, Expression<Func<TType, TProperty2?>> property2Selector,
            Expression<Func<TType, TProperty3?>> property3Selector, Expression<Func<TType, TProperty4?>> property4Selector,
            IEqualityComparer<(TProperty1?, TProperty2?, TProperty3?, TProperty4?)>? comparer = default)
        {
            RuleFor(collection => collection)
                .Custom((collection, context) =>
                {
                    if (collection is null)
                    {
                        return;
                    }

                    var (property1Name, func1) = IsUnique.GetPropertyNameAndFunc(property1Selector);
                    var (property2Name, func2) = IsUnique.GetPropertyNameAndFunc(property2Selector);
                    var (property3Name, func3) = IsUnique.GetPropertyNameAndFunc(property3Selector);
                    var (property4Name, func4) = IsUnique.GetPropertyNameAndFunc(property4Selector);

                    var duplicateValues = IsUnique.GetDuplicates(collection, item => (func1.Invoke(item), func2.Invoke(item), func3.Invoke(item), func4.Invoke(item)), comparer);

                    if (duplicateValues.Length != 0)
                    {
                        var values = duplicateValues.SelectToArray(value => $"{IsUnique.AsString(value.Item1)}, {IsUnique.AsString(value.Item2)}, {IsUnique.AsString(value.Item3)}, {IsUnique.AsString(value.Item4)}");

                        var failure = new ValidationFailure(
                                propertyName: $"{property1Name}, {property2Name}, {property3Name}, {property4Name}",
                                errorMessage: $"The combination of '{property1Name}', '{property2Name}', '{property3Name}' and '{property4Name}' must be unique.",
                            attemptedValue: values
                        )
                        {
                            ErrorCode = nameof(ValidationErrorCode.Duplicate)
                        };

                        context.AddFailure(failure);
                    }
                });
        }
    }

    internal static class IsUnique
    {
        public static (string, Func<TType, TProperty?>) GetPropertyNameAndFunc<TType, TProperty>(Expression<Func<TType, TProperty?>> propertySelector)
        {
            return (propertySelector.GetPropertyOrFieldMemberInfo().Name, propertySelector.Compile());
        }

        public static TProperty[] GetDuplicates<TType, TProperty>(IEnumerable<TType> collection, Func<TType, TProperty?> selector, IEqualityComparer<TProperty?>? comparer)
        {
            return [.. collection
                .GroupBy(selector.Invoke, comparer)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)];
        }

        public static string AsString<TType>(TType? value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}