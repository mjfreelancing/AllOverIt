using System;
using System.Linq.Expressions;
using AllOverIt.Expressions;

namespace AllOverIt.Patterns.Specification
{
    public sealed class OrNotLinqSpecification<TType> : BinaryLinqSpecification<TType>
    {
        public OrNotLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
            : base(leftSpecification, rightSpecification)
        {
        }

        public override Expression<Func<TType, bool>> AsExpression()
        {
            var leftExpression = LeftSpecification.AsExpression();
            var rightExpression = RightSpecification.AsExpression();

            return leftExpression.Or(rightExpression.Not());
        }
    }
}