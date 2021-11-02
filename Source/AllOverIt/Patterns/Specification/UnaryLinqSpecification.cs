using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>An abstract base class for all concrete LINQ-based unary specifications.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public abstract class UnaryLinqSpecification<TType> : LinqSpecification<TType>
    {
        /// <summary>The specification of the unary operation to apply to a candidate.</summary>
        protected ILinqSpecification<TType> Specification { get; }

        /// <summary>Constructor.</summary>
        /// <param name="specification">The unary specification to apply to a candidate.</param>
        protected UnaryLinqSpecification(ILinqSpecification<TType> specification)
        {
            Specification = specification.WhenNotNull(nameof(specification));
        }
    }
}