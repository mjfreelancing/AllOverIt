using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    public sealed class AndLinqSpecification<TType> : BinaryLinqSpecification<TType>
    {
        public AndLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification,
            bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        public override Expression<Func<TType, bool>> AsExpression()
        {
            var leftExpression = LeftSpecification.AsExpression();
            var rightExpression = RightSpecification.AsExpression();

            return leftExpression.And(rightExpression);
        }
    }
}