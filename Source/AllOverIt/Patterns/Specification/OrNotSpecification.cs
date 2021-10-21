namespace AllOverIt.Patterns.Specification
{
    public sealed class OrNotSpecification<TType> : BinarySpecification<TType>
    {
        public OrNotSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
            : base(leftSpecification, rightSpecification)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) || !RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}