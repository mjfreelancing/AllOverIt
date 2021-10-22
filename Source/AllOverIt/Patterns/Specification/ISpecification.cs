namespace AllOverIt.Patterns.Specification
{
    /// <summary>Defines an interface that allows for complex specifications to be built.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public interface ISpecification<in TType>
    {
        /// <summary>Applies a specification against a candidate subject.</summary>
        /// <param name="candidate">The subject to be tested against the specification.</param>
        /// <returns>True if the candidate satisfies the specification, otherwise false.</returns>
        bool IsSatisfiedBy(TType candidate);
    }
}