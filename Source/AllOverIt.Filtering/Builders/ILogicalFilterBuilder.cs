using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    /// <summary>Defines logical filter or specification operations that can be applied to a filter builder.</summary>
    /// <typeparam name="TType">The object type to apply the filter operation to.</typeparam>
    /// <typeparam name="TFilter">A custom filter type used for defining each operation or comparison in a specification.</typeparam>
    public interface ILogicalFilterBuilder<TType, TFilter> : IFilterSpecification<TType, TFilter>
      where TType : class
      where TFilter : class
    {
        /// <summary>Applies an <see cref="IStringFilterOperation"/> operation or comparison against a string property on a
        /// <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation">The filter operation to be applied to the current filter as a binary AND operation.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        /// <summary>Applies an <see cref="IBasicFilterOperation"/> or <see cref="IArrayFilterOperation"/> operation or comparison
        /// against a property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation">The filter operation to be applied to the current filter as a binary AND operation.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation, Action<OperationFilterOptions> options = default);

        /// <summary>Adds a specification to the filter builder. Multiple calls to this method will result in the subsequent filter operations
        /// being applied as a binary AND operation.</summary>
        /// <param name="specification">The specification to apply to the filter builder. Multiple calls to this method will result in the
        /// subsequent filter operations being applied as a binary AND operation.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification);

        /// <summary>Applies an <see cref="IStringFilterOperation"/> operation or comparison against a string property on a
        /// <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation">The filter operation to be applied to the current filter as a binary OR operation.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        /// <summary>Applies an <see cref="IBasicFilterOperation"/> or <see cref="IArrayFilterOperation"/> operation or comparison
        /// against a property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation">The filter operation to be applied to the current filter as a binary OR operation.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation, Action<OperationFilterOptions> options = default);

        /// <summary>Adds a specification to the filter builder. Multiple calls to this method will result in the subsequent filter operations
        /// being applied as a binary OR operation.</summary>
        /// <param name="specification">The specification to apply to the filter builder. Multiple calls to this method will result in the
        /// subsequent filter operations being applied as a binary OR operation.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification);
    }
}