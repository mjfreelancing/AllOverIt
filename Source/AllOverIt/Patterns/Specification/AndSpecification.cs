﻿namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs an AND operation between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class AndSpecification<TType> : BinarySpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification of the AND operation to apply to a candidate.</param>
        /// <param name="rightSpecification">The right specification of the AND operation to apply to a candidate.</param>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        public AndSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        /// <inheritdoc />
        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) && RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}