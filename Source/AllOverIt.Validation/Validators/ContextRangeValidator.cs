using AllOverIt.Assertion;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Validators;

namespace AllOverIt.Validation.Validators
{
    /// <summary>A base validation class that is used to validate a property value against two values available on the validation context.</summary>
    /// <typeparam name="TType">The model type containing the property to be validated.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TContext">The root context type.</typeparam>
    public abstract class ContextRangeValidator<TType, TProperty, TContext> : PropertyValidator<TType, TProperty>
        where TProperty : IComparable<TProperty>, IComparable
    {
        private readonly Func<TContext, TProperty> _fromValueResolver;
        private readonly Func<TContext, TProperty> _toValueResolver;

        /// <summary>Constructor.</summary>
        /// <param name="fromValueResolver">The resolver to obtain the lower limit from the validation context.</param>
        /// <param name="toValueResolver">The resolver to obtain the upper limit from the validation context.</param>
        protected ContextRangeValidator(Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
        {
            _fromValueResolver = fromValueResolver.WhenNotNull();
            _toValueResolver = toValueResolver.WhenNotNull();
        }

        /// <inheritdoc />
        public override bool IsValid(ValidationContext<TType> context, TProperty value)
        {
            var contextData = context.GetContextData<TType, TContext>();
            var fromValue = _fromValueResolver.Invoke(contextData);
            var toValue = _toValueResolver.Invoke(contextData);

            var isValid = IsValid(value, fromValue, toValue);

            if (!isValid)
            {
                context.MessageFormatter.AppendArgument("FromValue", fromValue);
                context.MessageFormatter.AppendArgument("ToValue", toValue);
            }

            return isValid;
        }

        /// <summary>Override in a concrete validator to indicate if the property value is valid compared to the two specified values.</summary>
        /// <param name="value">The property value.</param>
        /// <param name="fromValue">The lower limit value to compare to the property value.</param>
        /// <param name="toValue">The upper limit value to compare to the property value.</param>
        /// <returns><see langword="True" /> if the property value is valid, otherwise <see langword="False" />.</returns>
        protected abstract bool IsValid(TProperty value, TProperty fromValue, TProperty toValue);
    }
}