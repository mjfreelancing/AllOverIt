using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class UnarySpecificationDummy : UnarySpecification<int>
    {
        public ISpecification<int> Spec { get; }

        public int? Candidate { get; private set; }

        public UnarySpecificationDummy(ISpecification<int> specification, bool negate = false)
            : base(specification, negate)
        {
            Spec = specification;
        }

        protected override bool DoIsSatisfiedBy(int candidate)
        {
            Candidate = candidate;
            return true;
        }
    }
}