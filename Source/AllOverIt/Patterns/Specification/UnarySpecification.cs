using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    // Implements a base abstract class that tests a candidate against a unary specification.
    // TType is the candidate type to be tested.
    public abstract class UnarySpecification<TType> : Specification<TType>
    {
        protected ISpecification<TType> Specification { get; }

        protected UnarySpecification(ISpecification<TType> specification, bool negate = false)
          : base(negate)
        {
            Specification = specification.WhenNotNull(nameof(specification));
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return Specification.IsSatisfiedBy(candidate);
        }
    }
}