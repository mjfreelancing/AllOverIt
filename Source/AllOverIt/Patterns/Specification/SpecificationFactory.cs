namespace AllOverIt.Patterns.Specification
{
    // Implements a factory that creates specification instances.
    public sealed class SpecificationFactory : ISpecificationFactory
    {
        public ISpecification<TType> CreateSpecification<TSpecification, TType>() where TSpecification : ISpecification<TType>, new()
        {
            return new TSpecification();
        }
    }
}