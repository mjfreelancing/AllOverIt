using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete binary specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class BinarySpecification<TType> : Specification<TType>
    {
        /// <summary>The left specification of the binary operation to apply to a candidate.</summary>
        protected ISpecification<TType> LeftSpecification { get; }

        /// <summary>The right specification of the binary operation to apply to a candidate.</summary>
        protected ISpecification<TType> RightSpecification { get; }

        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        protected BinarySpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
        {
            LeftSpecification = leftSpecification.WhenNotNull(nameof(leftSpecification));
            RightSpecification = rightSpecification.WhenNotNull(nameof(rightSpecification));
        }
    }
}