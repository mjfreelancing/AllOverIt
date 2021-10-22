using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete unary specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class UnarySpecification<TType> : Specification<TType>
    {
        private readonly ISpecification<TType> _specification;

        /// <summary>Constructor.</summary>
        /// <param name="specification">The unary specification to apply to a candidate.</param>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        protected UnarySpecification(ISpecification<TType> specification, bool negate = false)
            : base(negate)
        {
            _specification = specification.WhenNotNull(nameof(specification));
        }

        /// <inheritdoc />
        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return _specification.IsSatisfiedBy(candidate);
        }
    }
}