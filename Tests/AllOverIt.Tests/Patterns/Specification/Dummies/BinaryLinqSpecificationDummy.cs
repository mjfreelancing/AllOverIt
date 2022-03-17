using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class BinaryLinqSpecificationDummy : BinaryLinqSpecification<int>
    {
        public ISpecification<int> Left { get; }
        public ISpecification<int> Right { get; }

        public BinaryLinqSpecificationDummy(ILinqSpecification<int> leftSpecification, ILinqSpecification<int> rightSpecification, bool result)
            : base(() => _ => result)
        {
            Left = leftSpecification;
            Right = rightSpecification;
        }
    }
}