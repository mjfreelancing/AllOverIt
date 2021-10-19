using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class SpecificationDummy : Specification<int>
    {
        public int Candidate { get; set; }

        public SpecificationDummy(bool negate)
            : base(negate)
        {
        }

        protected override bool DoIsSatisfiedBy(int candidate)
        {
            Candidate = candidate;

            return true;
        }
    }
}