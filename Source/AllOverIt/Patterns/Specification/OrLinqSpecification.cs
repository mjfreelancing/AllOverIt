using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs an OR operation between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class OrLinqSpecification<TType> : BinaryLinqSpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification of the OR operation to apply to a candidate.</param>
        /// <param name="rightSpecification">The right specification of the OR operation to apply to a candidate.</param>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        public OrLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification,
            bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        /// <inheritdoc />
        public override Expression<Func<TType, bool>> AsExpression()
        {
            var leftExpression = LeftSpecification.AsExpression();
            var rightExpression = RightSpecification.AsExpression();

            return leftExpression.Or(rightExpression);
        }
    }
}