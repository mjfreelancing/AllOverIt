namespace AllOverIt.Patterns.Specification.Types
{
    // A specification that determines if a specified nullable type has a value.
    public sealed class HasValue<TType> : ISpecification<TType?> where TType : struct
    {
        public bool IsSatisfiedBy(TType? candidate)
        {
            return candidate.HasValue;
        }
    }
}