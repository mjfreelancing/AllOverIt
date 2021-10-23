using System;
using System.Linq.Expressions;
using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class BinaryLinqSpecificationDummy : BinaryLinqSpecification<int>
    {
        public ISpecification<int> Left => LeftSpecification;
        public ISpecification<int> Right => RightSpecification;

        public BinaryLinqSpecificationDummy(ILinqSpecification<int> leftSpecification, ILinqSpecification<int> rightSpecification,
            bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        public override Expression<Func<int, bool>> AsExpression()
        {
            return _ => true;
        }
    }
}