using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal class UnaryLinqSpecificationDummy : UnaryLinqSpecification<int>
    {
        public ILinqSpecification<int> Spec { get; }

        public UnaryLinqSpecificationDummy(ILinqSpecification<int> specification)
            : base(() => _ => true)
        {
            Spec = specification;
        }
    }
}