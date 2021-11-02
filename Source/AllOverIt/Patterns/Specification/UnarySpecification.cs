using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete unary specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class UnarySpecification<TType> : Specification<TType>
    {
        private readonly ISpecification<TType> _specification;

        /// <summary>Constructor.</summary>
        /// <param name="specification">The unary specification to apply to a candidate.</param>
        protected UnarySpecification(ISpecification<TType> specification)
        {
            _specification = specification.WhenNotNull(nameof(specification));
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TType candidate)
        {
            return _specification.IsSatisfiedBy(candidate);
        }
    }
}