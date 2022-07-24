using AllOverIt.Patterns.Specification;
using System.Linq;

namespace AllOverIt.Filtering.Builders
{
    /// <summary>Defines a filter that can be expressed as a specification.</summary>
    /// <typeparam name="TType">The object type to apply the filter operation to.</typeparam>
    /// <typeparam name="TFilter">A custom filter type used for defining each operation or comparison in a specification.</typeparam>
    public interface IFilterSpecification<TType, TFilter>
      where TType : class
      where TFilter : class
    {
        /// <summary>Gets the state of a filter builder as an invokable specification. The specification can also be applied to an <see cref="IQueryable{T}"/>
        /// Where() statement.</summary>
        ILinqSpecification<TType> AsSpecification();
    }
}