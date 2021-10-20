namespace AllOverIt.Patterns.Specification
{
    // Defines an interface that allows for complex specifications to be built.
    // TType is the candidate type the specification applies to.
    public interface ISpecification<in TType>
    {
        bool IsSatisfiedBy(TType candidate);
    }
}