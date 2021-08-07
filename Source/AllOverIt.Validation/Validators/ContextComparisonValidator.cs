using AllOverIt.Helpers;
using AllOverIt.Validation.Extensions;
using FluentValidation;
using FluentValidation.Validators;
using System;

namespace AllOverIt.Validation.Validators
{
    // A base validation class that obtains the comparison context for use by a rule at the time of invocation.
    public abstract class ContextComparisonValidator<TType, TProperty, TContext> : PropertyValidator<TType, TProperty>
        where TProperty : IComparable<TProperty>, IComparable
    {
        private readonly Func<TContext, TProperty> _contextValueResolver;

        public abstract ValidationErrorCode ErrorCode { get; }

        public ContextComparisonValidator(Func<TContext, TProperty> contextValueResolver)
        {
            _ = contextValueResolver.WhenNotNull(nameof(contextValueResolver));

            _contextValueResolver = contextValueResolver;
        }

        public override bool IsValid(ValidationContext<TType> context, TProperty value)
        {
            var contextData = context.GetContextData<TType, TContext>();
            var comparisonValue = _contextValueResolver.Invoke(contextData);

            var isValid = IsValid(value, comparisonValue);

            if (!isValid)
            {
                context.MessageFormatter.AppendArgument("ComparisonValue", comparisonValue);
            }

            return isValid;
        }

        protected abstract bool IsValid(TProperty value, TProperty comparisonValue);
    }
}