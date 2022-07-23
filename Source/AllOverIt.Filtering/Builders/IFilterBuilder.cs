using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;
using AllOverIt.Filtering.Extensions;

namespace AllOverIt.Filtering.Builders
{
    /// <summary>Defines a queryable filter builder for a specified <typeparamref name="TType"/> object and filter type. This builder can
    /// be used for building any general purpose queryable specification but is typically used in conjunction with
    /// <see cref="QueryableExtensions.ApplyFilter{TType, TFilter}(System.Linq.IQueryable{TType}, TFilter, Action{IFilterSpecificationBuilder{TType, TFilter}, IFilterBuilder{TType, TFilter}}, QueryFilterOptions)"/></summary>
    /// <typeparam name="TType">The object type to apply the specification to.</typeparam>
    /// <typeparam name="TFilter">A custom filter type used for defining each operation or comparison in the specification.</typeparam>
    public interface IFilterBuilder<TType, TFilter>
       where TType : class
       where TFilter : class
    {
        /// <summary>
        /// <para>
        /// Gets the current logical expression to cater for additional of <see cref="Where(ILinqSpecification{TType})"/> or
        /// <see cref="ILogicalFilterBuilder{TType, TFilter}.And(ILinqSpecification{TType})"/> or <see cref="ILogicalFilterBuilder{TType, TFilter}.Or(ILinqSpecification{TType})"/>
        /// calls (or one of their overloads).
        /// </para>
        /// <para>
        /// This property returns the current filter state. Subsequent calls to <see cref="ILogicalFilterBuilder{TType, TFilter}.And(ILinqSpecification{TType})"/> or
        /// <see cref="ILogicalFilterBuilder{TType, TFilter}.Or(ILinqSpecification{TType})"/> (or one of their overloads) will apply the new expression as a binary operation
        /// against <see cref="Current"/>.
        /// </para>
        /// </summary>
        ILogicalFilterBuilder<TType, TFilter> Current { get; }

        /// <summary>Gets the state of the filter builder as an invokable specification.</summary>
        ILinqSpecification<TType> AsSpecification();

        /// <summary>Adds a filter operation to the filter builder, applying it is an equivalent specification. Multiple calls to this method
        /// will result in the subsequent filter operations being applied as a binary AND operation.</summary>
        /// <param name="propertyExpression">The expression specifying a string property on the <typeparamref name="TType"/> to apply the filter operation to.</param>
        /// <param name="operation">An <see cref="IStringFilterOperation"/> filter operation to apply.</param>
        /// <param name="options">An optional action to configure options that control how the specifications are built.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        /// <summary>Adds a filter operation to the filter builder, applying it is an equivalent specification. Multiple calls to this method
        /// will result in the subsequent filter operations being applied as a binary AND operation.</summary>
        /// <typeparam name="TProperty">The property type for the specified property expression.</typeparam>
        /// <param name="propertyExpression">The expression specifying a property on the <typeparamref name="TType"/> to apply the filter operation to.</param>
        /// <param name="operation">An <see cref="IBasicFilterOperation"/> or <see cref="IArrayFilterOperation"/> filter operation to apply.</param>
        /// <param name="options">An optional action to configure options that control how the specifications are built.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        /// <summary>Adds a specification to the filter builder. Multiple calls to this method will result in the subsequent filter operations
        /// being applied as a binary AND operation.</summary>
        /// <param name="specification">The specification to apply to the filter builder. Multiple calls to this method will result in the
        /// subsequent filter operations being applied as a binary AND operation.</param>
        /// <returns>A reference to the current filter builder so additional logical operations can be applied.</returns>
        ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification);
    }
}