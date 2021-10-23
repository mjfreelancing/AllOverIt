namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs a logical AND operation between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class AndSpecification<TType> : BinarySpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        public AndSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
            : base(leftSpecification, rightSpecification)
        {
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) && RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}