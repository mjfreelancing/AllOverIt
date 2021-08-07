﻿using System;

namespace AllOverIt.Validation.Validators
{
    public sealed class LessThanOrEqualToContextValidator<TType, TProperty, TContext> : ContextComparisonValidator<TType, TProperty, TContext>
        where TProperty : IComparable<TProperty>, IComparable
    {
        public override string Name => "LessThanOrEqualToContextValidator";
        public override ValidationErrorCode ErrorCode => ValidationErrorCode.OutOfRange;

        public LessThanOrEqualToContextValidator(Func<TContext, TProperty> contextValueResolver)
            : base(contextValueResolver)
        {
        }

        protected override bool IsValid(TProperty value, TProperty comparisonValue)
        {
            return value.CompareTo(comparisonValue) <= 0;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "'{PropertyName}' must be less than or equal to '{ComparisonValue}'.";
        }
    }
}