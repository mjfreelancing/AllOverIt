namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class SpecificationBase<TType>
    {
        private readonly bool _negate;

        /// <summary>Constructor.</summary>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        protected SpecificationBase(bool negate = false)
        {
            _negate = negate; 
        }

        /// <summary>Implemented by concrete classes to perform the required specification test.</summary>
        /// <param name="candidate">The subject to be tested against the specification.</param>
        /// <returns>True if the candidate satisfies the specification, otherwise false.</returns>
        protected abstract bool DoIsSatisfiedBy(TType candidate);

        /// <summary>Required in combination with operator & and | to support operator &amp;&amp; and ||.</summary>
        public static bool operator true(SpecificationBase<TType> _)
        {
            return false;
        }

        /// <summary>Required in combination with operator & and | to support operator &amp;&amp; and ||.</summary>
        public static bool operator false(SpecificationBase<TType> _)
        {
            return false;
        }

        /// <summary>Invokes the specification against the provided candidate.</summary>
        /// <param name="candidate">The subject to be tested against the specification.</param>
        /// <returns>True if the candidate satisfies the specification, otherwise false.</returns>
        public bool IsSatisfiedBy(TType candidate)
        {
            var result = DoIsSatisfiedBy(candidate);

            return _negate
                ? !result
                : result;
        }
    }
}