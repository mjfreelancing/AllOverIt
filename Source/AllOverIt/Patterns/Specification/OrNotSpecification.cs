namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs an OR NOT operation between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class OrNotSpecification<TType> : BinarySpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification of the OR NOT operation to apply to a candidate.</param>
        /// <param name="rightSpecification">The right specification of the OR NOT operation to apply to a candidate.</param>
        public OrNotSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
            : base(leftSpecification, rightSpecification)
        {
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) || !RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}