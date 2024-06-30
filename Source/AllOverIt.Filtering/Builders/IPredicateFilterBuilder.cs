using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    /// <summary>Defines predicate filter or specification operations that can be applied to a filter builder.</summary>
    /// <typeparam name="TType">The object type to apply the filter operation to.</typeparam>
    /// <typeparam name="TFilter">A custom filter type used for defining each operation or comparison in a specification.</typeparam>
    public interface IPredicateFilterBuilder<TType, TFilter> : IFilterSpecification<TType, TFilter>
       where TType : class
       where TFilter : class
    {
        /// <summary>Adds a filter operation to the filter builder, applying it is an equivalent specification. Multiple calls to this method
        /// will result in the subsequent filter operations being applied as a binary AND operation.</summary>
        /// <param name="propertyExpression">The expression specifying a string property on the <typeparamref name="TType"/> to apply the filter operation to.</param>
        /// <param name="operation">An <see cref="IStringFilterOperation"/> filter operation to apply.</param>
        /// <param name="options">An optional action to configure options that control how the specifications are built.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions>? options = default);

        /// <summary>Adds a filter operation to the filter builder, applying it is an equivalent specification. Multiple calls to this method
        /// will result in the subsequent filter operations being applied as a binary AND operation.</summary>
        /// <typeparam name="TProperty">The property type for the specified property expression.</typeparam>
        /// <param name="propertyExpression">The expression specifying a property on the <typeparamref name="TType"/> to apply the filter operation to.</param>
        /// <param name="operation">An <see cref="IBasicFilterOperation"/> or <see cref="IArrayFilterOperation"/> filter operation to apply.</param>
        /// <param name="options">An optional action to configure options that control how the specifications are built.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation,
            Action<OperationFilterOptions>? options = default);

        /// <summary>Adds a specification to the filter builder. Multiple calls to this method will result in the subsequent filter operations
        /// being applied as a binary AND operation.</summary>
        /// <param name="specification">The specification to apply to the filter builder. Multiple calls to this method will result in the
        /// subsequent filter operations being applied as a binary AND operation.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification);
    }
}