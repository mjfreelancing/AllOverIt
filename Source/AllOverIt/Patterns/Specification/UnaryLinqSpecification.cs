using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete LINQ-based unary specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class UnaryLinqSpecification<TType> : LinqSpecification<TType>
    {
        protected ILinqSpecification<TType> Specification { get; }

        /// <summary>Constructor.</summary>
        /// <param name="specification">The unary specification to apply to a candidate.</param>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        protected UnaryLinqSpecification(ILinqSpecification<TType> specification, bool negate = false)
            : base(negate)
        {
            Specification = specification.WhenNotNull(nameof(specification));
        }
    }
}