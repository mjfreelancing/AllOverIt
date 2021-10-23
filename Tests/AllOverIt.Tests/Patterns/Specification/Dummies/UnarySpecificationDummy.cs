using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class UnarySpecificationDummy : UnarySpecification<int>
    {
        private readonly bool _result;
        public ISpecification<int> Spec { get; }

        public int? Candidate { get; private set; }

        public UnarySpecificationDummy(ISpecification<int> specification, bool result)
            : base(specification)
        {
            Spec = specification;
            _result = result;
        }

        public override bool IsSatisfiedBy(int candidate)
        {
            Candidate = candidate;
            return _result;
        }
    }
}