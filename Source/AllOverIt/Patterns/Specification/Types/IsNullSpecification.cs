namespace AllOverIt.Patterns.Specification.Types
{
    // A specification that determines if a specified object is null.
    public sealed class IsNullSpecification<TType> : ISpecification<TType> where TType : class
    {
        public bool IsSatisfiedBy(TType candidate)
        {
            return candidate == null;
        }
    }
}