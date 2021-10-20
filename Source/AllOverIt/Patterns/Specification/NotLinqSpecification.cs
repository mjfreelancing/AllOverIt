using System;
using System.Linq.Expressions;
using AllOverIt.Expressions;

namespace AllOverIt.Patterns.Specification
{
    public sealed class NotLinqSpecification<TType> : UnaryLinqSpecification<TType>
    {
        public NotLinqSpecification(ILinqSpecification<TType> specification)
            : base(specification, true)
        {
        }

        public override Expression<Func<TType, bool>> AsExpression()
        {
            return Specification.AsExpression().Not();
        }
    }
}