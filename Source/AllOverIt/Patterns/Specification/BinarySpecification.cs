using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    // Implements a base abstract class that tests a candidate against a binary specification.
    // TType is the candidate type to be tested.
    public abstract class BinarySpecification<TType> : Specification<TType>
    {
        protected ISpecification<TType> LeftSpecification { get; }
        protected ISpecification<TType> RightSpecification { get; }

        protected BinarySpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification,
            bool negate = false)
            : base(negate)
        {
            LeftSpecification = leftSpecification.WhenNotNull(nameof(leftSpecification));
            RightSpecification = rightSpecification.WhenNotNull(nameof(rightSpecification));
        }
    }
}