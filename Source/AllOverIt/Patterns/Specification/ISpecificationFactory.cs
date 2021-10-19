namespace AllOverIt.Patterns.Specification
{
    // An interface defining a factory that creates specification instances.
    public interface ISpecificationFactory
    {
        // Creates a new specification instance of type TSpecification (must implement ISpecification and have a default constructor).
        // TType is the candidate type to be tested.
        ISpecification<TType> CreateSpecification<TSpecification, TType>() where TSpecification : ISpecification<TType>, new();
    }
}