namespace AllOverIt.Patterns.Specification
{
    public abstract class SpecificationBase<TType>
    {
        private readonly bool _negate;

        protected SpecificationBase(bool negate = false)
        {
            _negate = negate; 
        }

        protected abstract bool DoIsSatisfiedBy(TType candidate);

        // required in combination with operator & and | to support && and ||
        public static bool operator true(SpecificationBase<TType> _)
        {
            return false;
        }

        // required in combination with operator & and | to support && and ||
        public static bool operator false(SpecificationBase<TType> _)
        {
            return false;
        }

        public bool IsSatisfiedBy(TType candidate)
        {
            var result = DoIsSatisfiedBy(candidate);

            return _negate
                ? !result
                : result;
        }
    }
}