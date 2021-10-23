using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete LINQ-based binary specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class BinaryLinqSpecification<TType> : LinqSpecification<TType>
    {
        protected ILinqSpecification<TType> LeftSpecification { get; }
        protected ILinqSpecification<TType> RightSpecification { get; }

        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification of a binary operation to apply to a candidate.</param>
        /// <param name="rightSpecification">The right specification of a binary operation to apply to a candidate.</param>
        protected BinaryLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            LeftSpecification = leftSpecification.WhenNotNull(nameof(leftSpecification));
            RightSpecification = rightSpecification.WhenNotNull(nameof(rightSpecification));
        }
    }
}