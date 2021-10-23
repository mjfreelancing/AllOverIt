using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class SpecificationDummy : Specification<int>
    {
        private readonly bool _result;
        public int Candidate { get; private set; }

        public SpecificationDummy(bool result)
        {
            _result = result;
        }

        public override bool IsSatisfiedBy(int candidate)
        {
            Candidate = candidate;

            return _result;
        }
    }
}