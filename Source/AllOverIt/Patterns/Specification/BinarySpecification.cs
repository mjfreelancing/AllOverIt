﻿using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete binary specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class BinarySpecification<TType> : Specification<TType>
    {
        protected ISpecification<TType> LeftSpecification { get; }
        protected ISpecification<TType> RightSpecification { get; }

        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification of a binary operation to apply to a candidate.</param>
        /// <param name="rightSpecification">The right specification of a binary operation to apply to a candidate.</param>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        protected BinarySpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification,
            bool negate = false)
            : base(negate)
        {
            LeftSpecification = leftSpecification.WhenNotNull(nameof(leftSpecification));
            RightSpecification = rightSpecification.WhenNotNull(nameof(rightSpecification));
        }
    }
}