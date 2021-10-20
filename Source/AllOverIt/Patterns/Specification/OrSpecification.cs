using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    // Determines if a specified candidate meets the criteria defined by either of two specifications.
    // TType is the candidate type to be tested.
    public sealed class OrSpecification<TType> : BinarySpecification<TType>
    {
        public OrSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) || RightSpecification.IsSatisfiedBy(candidate);
        }
    }


    public sealed class OrLinqSpecification<TType> : BinaryLinqSpecification<TType>
    {
        public OrLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        public override Expression<Func<TType, bool>> AsExpression()
        {
            var leftExpression = LeftSpecification.AsExpression();
            var rightExpression = RightSpecification.AsExpression();

            return leftExpression.Or(rightExpression);
        }
    }


}