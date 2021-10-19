namespace AllOverIt.Patterns.Specification
{
    // Determines if a specified candidate meets the criteria defined by either of two specifications.
    // TType is the candidate type to be tested.
    public sealed class OrSpecification<TType> : BinarySpecification<TType>
    {
        public OrSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) || RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}