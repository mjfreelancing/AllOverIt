using AllOverIt.Expressions;
using AllOverIt.Extensions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    // Implements a unary specification that negates the result of a specified specification.
    // TType is the candidate type to be tested.
    public sealed class NotSpecification<TType> : UnarySpecification<TType>
    {
        public NotSpecification(ISpecification<TType> specification)
            : base(specification, true)
        {
        }
    }



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