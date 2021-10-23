namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs an AND NOT operation (left &amp;&amp; !right) between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class AndNotSpecification<TType> : BinarySpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification of the AND NOT operation to apply to a candidate.</param>
        /// <param name="rightSpecification">The right specification of the AND NOT operation to apply to a candidate.</param>
        public AndNotSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
            : base(leftSpecification, rightSpecification)
        {
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) && !RightSpecification.IsSatisfiedBy(candidate);
        }
    }
}