namespace AllOverIt.Patterns.Specification
{
    public sealed class AndNotSpecification<TType> : BinarySpecification<TType>
    {
        public AndNotSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
            : base(leftSpecification, rightSpecification)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) && !RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}