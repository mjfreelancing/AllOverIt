using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>Defines an interface that allows for complex Expression-based specifications to be built.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public interface ILinqSpecification<TType> : ISpecification<TType>
    {
        /// <summary>Gets an Expression that describes the specification.</summary>
        /// <returns>An Expression that describes the specification.</returns>
        Expression<Func<TType, bool>> Expression { get; }
    }
}