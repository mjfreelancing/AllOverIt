namespace AllOverIt.Patterns.Specification
{
    // An abstract specification class that determines if an object meets the criteria defined by the specification.
    // TType is the candidate type to be tested.
    public abstract class Specification<TType> : ISpecification<TType>
    {
        // Indicates if the result of the specification's criteria should be negated.
        protected bool Negate { get; }

        protected Specification(bool negate = false)
        {
            Negate = negate;
        }

        public bool IsSatisfiedBy(TType candidate)
        {
            var result = DoIsSatisfiedBy(candidate);

            return Negate
              ? !result
              : result;
        }

        protected abstract bool DoIsSatisfiedBy(TType candidate);
    }
}