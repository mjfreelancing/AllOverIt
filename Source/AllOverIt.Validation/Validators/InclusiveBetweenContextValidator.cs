using System;

namespace AllOverIt.Validation.Validators
{
    /// <summary>A range validator that compares a property value against an inclusive upper and lower limit stored on the validation context.</summary>
    /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TContext">The root context type.</typeparam>
    public sealed class InclusiveBetweenContextValidator<TType, TProperty, TContext> : ContextRangeValidator<TType, TProperty, TContext>
        where TProperty : IComparable<TProperty>, IComparable
    {
        /// <inheritdoc />
        public override string Name => "InclusiveBetweenContextValidator";

        /// <inheritdoc />
        public InclusiveBetweenContextValidator(Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
            : base(fromValueResolver, toValueResolver)
        {
        }

        /// <inheritdoc />
        protected override bool IsValid(TProperty value, TProperty fromValue, TProperty toValue)
        {
            return value.CompareTo(fromValue) >= 0 && value.CompareTo(toValue) <= 0;
        }

        /// <inheritdoc />
        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "'{PropertyName}' must be between {FromValue} and {ToValue} (inclusive).";
        }
    }
}