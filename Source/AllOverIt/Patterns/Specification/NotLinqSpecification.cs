using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs a NOT operation between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class NotLinqSpecification<TType> : UnaryLinqSpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="specification">The specification of the NOT operation to apply to a candidate.</param>
        public NotLinqSpecification(ILinqSpecification<TType> specification)
            : base(specification, true)
        {
        }

        /// <inheritdoc />
        public override Expression<Func<TType, bool>> AsExpression()
        {
            return Specification.AsExpression().Not();
        }
    }
}