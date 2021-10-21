namespace AllOverIt.Patterns.Specification
{
    // Determines if a specified candidate meets the criteria defined by two specifications.
    // TType is the candidate type to be tested.
    public sealed class AndSpecification<TType> : BinarySpecification<TType>
    {
        public AndSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) && RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}