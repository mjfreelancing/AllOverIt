using AllOverIt.Assertion;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Validators;

namespace AllOverIt.Validation.Validators
{
    /// <summary>A base validation class that obtains the comparison context for use by a rule at the time of invocation.</summary>
    /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TContext">The root context type.</typeparam>
    public abstract class ContextComparisonValidator<TType, TProperty, TContext> : PropertyValidator<TType, TProperty>
        where TProperty : IComparable<TProperty>, IComparable
    {
        private readonly Func<TContext, TProperty> _valueResolver;

        /// <summary>Constructor.</summary>
        /// <param name="valueResolver">The resolver to obtain the comparison value from the validation context.</param>
        protected ContextComparisonValidator(Func<TContext, TProperty> valueResolver)
        {
            _valueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
        }

        /// <inheritdoc />
        public override bool IsValid(ValidationContext<TType> context, TProperty value)
        {
            var contextData = context.GetContextData<TType, TContext>();
            var comparisonValue = _valueResolver.Invoke(contextData);

            var isValid = IsValid(value, comparisonValue);

            if (!isValid)
            {
                context.MessageFormatter.AppendArgument("ComparisonValue", comparisonValue);
            }

            return isValid;
        }

        /// <summary>Override in a concrete validator to indicate if the property value is valid compared to the specified comparison value.</summary>
        /// <param name="value">The property value.</param>
        /// <param name="comparisonValue">The value to compare to the property value.</param>
        /// <returns><see langword="true" /> if the property value is valid, otherwise <see langword="false" />.</returns>
        protected abstract bool IsValid(TProperty value, TProperty comparisonValue);
    }
}