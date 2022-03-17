using AllOverIt.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs a logical NOT operation on a provided expression.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class NotLinqSpecification<TType> : LinqSpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="specification">The specification applied against a candidate.</param>
        public NotLinqSpecification(ILinqSpecification<TType> specification)
            : base(() => specification.Expression.Not())
        {
        }
    }
}