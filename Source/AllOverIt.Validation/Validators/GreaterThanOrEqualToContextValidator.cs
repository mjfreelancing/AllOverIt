using System;

namespace AllOverIt.Validation.Validators
{
    public sealed class GreaterThanOrEqualToContextValidator<TType, TProperty, TContext> : ContextComparisonValidator<TType, TProperty, TContext>
        where TProperty : IComparable<TProperty>, IComparable
    {
        public override string Name => "GreaterThanOrEqualToContextValidator";

        public GreaterThanOrEqualToContextValidator(Func<TContext, TProperty> valueResolver)
            : base(valueResolver)
        {
        }

        protected override bool IsValid(TProperty value, TProperty comparisonValue)
        {
            return value.CompareTo(comparisonValue) >= 0;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "'{PropertyName}' must be greater than or equal to {ComparisonValue}.";
        }
    }
}