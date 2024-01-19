namespace AllOverIt.Patterns.Specification
{
    // Note: tested indirectly via SpecificationFixture

    /// <summary>An abstract base class for all concrete specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class SpecificationBase<TType>
    {
        /// <summary>Implemented by concrete classes to perform the required specification test.</summary>
        /// <param name="candidate">The subject to be tested against the specification.</param>
        /// <returns><see langword="true" /> if the candidate satisfies the specification, otherwise <see langword="false" />.</returns>
        public abstract bool IsSatisfiedBy(TType candidate);

        /// <summary>Required in combination with operator &amp; and | to support operator &amp;&amp; and ||.</summary>
        public static bool operator true(SpecificationBase<TType> _)
        {
            return false;
        }

        /// <summary>Required in combination with operator &amp; and | to support operator &amp;&amp; and ||.</summary>
        public static bool operator false(SpecificationBase<TType> _)
        {
            return false;
        }
    }
}