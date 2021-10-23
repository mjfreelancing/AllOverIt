using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class BinaryLinqSpecificationDummy : BinaryLinqSpecification<int>
    {
        private readonly bool _result;
        public ISpecification<int> Left => LeftSpecification;
        public ISpecification<int> Right => RightSpecification;

        public BinaryLinqSpecificationDummy(ILinqSpecification<int> leftSpecification, ILinqSpecification<int> rightSpecification,
            bool result)
            : base(leftSpecification, rightSpecification)
        {
            _result = result;
        }

        public override Expression<Func<int, bool>> AsExpression()
        {
            return _ => _result;
        }
    }
}