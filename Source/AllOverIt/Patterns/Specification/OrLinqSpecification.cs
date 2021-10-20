using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    public sealed class OrLinqSpecification<TType> : BinaryLinqSpecification<TType>
    {
        public OrLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification,
            bool negate = false)
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