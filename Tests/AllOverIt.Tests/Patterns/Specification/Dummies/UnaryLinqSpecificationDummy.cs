using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class UnaryLinqSpecificationDummy : UnaryLinqSpecification<int>
    {
        public ILinqSpecification<int> Spec { get; }

        public UnaryLinqSpecificationDummy(ILinqSpecification<int> specification)
            : base(specification)
        {
            Spec = specification;
        }

        public override Expression<Func<int, bool>> AsExpression()
        {
            return _ => true;
        }
    }
}