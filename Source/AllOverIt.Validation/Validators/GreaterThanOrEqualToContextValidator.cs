using System;

namespace AllOverIt.Validation.Validators
{
    /// <summary>A validator that determines if a property value is greater than or equal to a comparison value stored on the validation context.</summary>
    /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TContext">The root context type.</typeparam>
    public sealed class GreaterThanOrEqualToContextValidator<TType, TProperty, TContext> : ContextComparisonValidator<TType, TProperty, TContext>
        where TProperty : IComparable<TProperty>, IComparable
    {
        /// <inheritdoc />
        public override string Name => "GreaterThanOrEqualToContextValidator";

        /// <summary>Constructor.</summary>
        /// <param name="valueResolver">The resolver to obtain the comparison value from the validation context.</param>
        public GreaterThanOrEqualToContextValidator(Func<TContext, TProperty> valueResolver)
            : base(valueResolver)
        {
        }

        /// <inheritdoc />
        protected override bool IsValid(TProperty value, TProperty comparisonValue)
        {
            return value.CompareTo(comparisonValue) >= 0;
        }

        /// <inheritdoc />
        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "'{PropertyName}' must be greater than or equal to {ComparisonValue}.";
        }
    }
}