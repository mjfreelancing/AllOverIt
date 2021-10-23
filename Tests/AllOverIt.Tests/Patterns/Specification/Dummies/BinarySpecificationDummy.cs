using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class BinarySpecificationDummy : BinarySpecification<int>
    {
        private readonly bool _result;
        public ISpecification<int> Left => LeftSpecification;
        public ISpecification<int> Right => RightSpecification;
        public int? Candidate { get; private set; }

        public BinarySpecificationDummy(ISpecification<int> leftSpecification, ISpecification<int> rightSpecification, bool result)
            : base(leftSpecification, rightSpecification)
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