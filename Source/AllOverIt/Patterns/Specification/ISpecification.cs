namespace AllOverIt.Patterns.Specification
{
    // Defines an interface that allows for complex specifications to be built.
    // TType is the candidate type the specification applies to.
    public interface ISpecification<in TType>
    {
        // Determines if a candidate instance meets the criteria defined by the specification.
        bool IsSatisfiedBy(TType candidate);
    }
}