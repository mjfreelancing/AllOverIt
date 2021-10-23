namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs a logical NOT operation between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class NotSpecification<TType> : UnarySpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="specification">The specification applied against a candidate.</param>
        public NotSpecification(ISpecification<TType> specification)
            : base(specification)
        {
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TType candidate)
        {
            return !base.IsSatisfiedBy(candidate);
        }
    }
}