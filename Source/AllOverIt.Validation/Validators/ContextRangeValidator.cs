using System;
using AllOverIt.Helpers;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Validators;

namespace AllOverIt.Validation.Validators
{
    public abstract class ContextRangeValidator<TType, TProperty, TContext> : PropertyValidator<TType, TProperty>
        where TProperty : IComparable<TProperty>, IComparable
    {
        private readonly Func<TContext, TProperty> _fromValueResolver;
        private readonly Func<TContext, TProperty> _toValueResolver;

        protected ContextRangeValidator(Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
        {
            _fromValueResolver = fromValueResolver.WhenNotNull(nameof(fromValueResolver));
            _toValueResolver = toValueResolver.WhenNotNull(nameof(toValueResolver));
        }

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

        protected abstract bool IsValid(TProperty value, TProperty fromValue, TProperty toValue);
    }
}