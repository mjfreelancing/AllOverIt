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
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        protected BinaryLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification,
            bool negate = false)
            : base(negate)
        {
            LeftSpecification = leftSpecification.WhenNotNull(nameof(leftSpecification));
            RightSpecification = rightSpecification.WhenNotNull(nameof(rightSpecification));
        }
    }
}