using System;

namespace AllOverIt.Validation.Validators
{
    public sealed class InclusiveBetweenContextValidator<TType, TProperty, TContext> : ContextRangeValidator<TType, TProperty, TContext>
        where TProperty : IComparable<TProperty>, IComparable
    {
        public override string Name => "InclusiveBetweenContextValidator";

        public InclusiveBetweenContextValidator(Func<TContext, TProperty> fromValueResolver, Func<TContext, TProperty> toValueResolver)
            : base(fromValueResolver, toValueResolver)
        {
        }

        protected override bool IsValid(TProperty value, TProperty fromValue, TProperty toValue)
        {
            return value.CompareTo(fromValue) >= 0 && value.CompareTo(toValue) <= 0;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "'{PropertyName}' must be between {FromValue} and {ToValue} (inclusive).";
        }
    }
}