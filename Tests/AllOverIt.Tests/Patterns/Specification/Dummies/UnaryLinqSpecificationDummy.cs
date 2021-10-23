using System;
using System.Linq.Expressions;
using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class UnaryLinqSpecificationDummy : UnaryLinqSpecification<int>
    {
        public ILinqSpecification<int> Spec { get; }

        public UnaryLinqSpecificationDummy(ILinqSpecification<int> specification, bool negate = false)
            : base(specification, negate)
        {
            Spec = specification;
        }

        public override Expression<Func<int, bool>> AsExpression()
        {
            return _ => true;
        }
    }
}