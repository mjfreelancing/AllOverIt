namespace AllOverIt.Patterns.Specification.Types
{
    // A specification that determines if a specified object is not null.
    public sealed class IsNotNull<TType> : ISpecification<TType> where TType : class
    {
        public bool IsSatisfiedBy(TType candidate)
        {
            return candidate != null;
        }
    }
}