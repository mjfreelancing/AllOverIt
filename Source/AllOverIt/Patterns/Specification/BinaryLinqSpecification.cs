using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    public abstract class BinaryLinqSpecification<TType> : LinqSpecification<TType>
    {
        protected ILinqSpecification<TType> LeftSpecification { get; }
        protected ILinqSpecification<TType> RightSpecification { get; }

        protected BinaryLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification,
            bool negate = false)
            : base(negate)
        {
            LeftSpecification = leftSpecification.WhenNotNull(nameof(leftSpecification));
            RightSpecification = rightSpecification.WhenNotNull(nameof(rightSpecification));
        }
    }
}