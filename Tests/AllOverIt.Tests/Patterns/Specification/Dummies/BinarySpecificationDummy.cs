using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class BinarySpecificationDummy : BinarySpecification<int>
    {
        public ISpecification<int> Left => LeftSpecification;
        public ISpecification<int> Right => RightSpecification;
        public int? Candidate { get; private set; }

        public BinarySpecificationDummy(ISpecification<int> leftSpecification, ISpecification<int> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        protected override bool DoIsSatisfiedBy(int candidate)
        {
            Candidate = candidate;
            return true;
        }
    }
}