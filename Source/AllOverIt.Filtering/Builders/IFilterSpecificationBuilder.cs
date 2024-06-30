using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    /// <summary>Defines a builder that can create an <see cref="ILinqSpecification{TType}" /> for a given object and filter type. The specification
    /// can be used for all general queryable filtering, but is typically used in via the
    /// <see cref="QueryableExtensions.ApplyFilter{TType, TFilter}(IQueryable{TType}, TFilter, Action{IFilterSpecificationBuilder{TType, TFilter}, IFilterBuilder{TType, TFilter}}, DefaultQueryFilterOptions)"/>
    /// method.</summary>
    /// <typeparam name="TType">The object type to apply the specification to.</typeparam>
    /// <typeparam name="TFilter">A custom filter type used for defining each operation or comparison in the specification.</typeparam>
    public interface IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class
    {
        #region Create

        /// <summary>Create a specification for a single <see cref="IStringFilterOperation"/> operation or comparison against a string property on a
        /// <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the specification operation against.</param>
        /// <param name="operation">The operation or comparison to be performed.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>A specification for performing an operation or comparison against a specified string property on a <typeparamref name="TType"/>
        /// instance.</returns>
        ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions>? options = default);

        /// <summary>Create a specification for a single <see cref="IBasicFilterOperation"/> (or <see cref="IArrayFilterOperation"/>) operation
        /// or comparison against a property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the specification operation against.</param>
        /// <param name="operation">The operation or comparison to be performed.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>A specification for performing an operation or comparison against a specified property on a <typeparamref name="TType"/>
        /// instance.</returns>
        ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation,
            Action<OperationFilterOptions>? options = default);

        #endregion

        #region AND

        /// <summary>Create a specification that ANDs an <see cref="IBasicFilterOperation"/> (or <see cref="IArrayFilterOperation"/>)
        /// with an <see cref="IStringFilterOperation"/> operation or comparison against a string property on a
        /// <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An AND combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IBasicFilterOperation<string>> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default);

        /// <summary>Create a specification that ANDs an <see cref="IStringFilterOperation"/> with an <see cref="IBasicFilterOperation"/>
        /// (or <see cref="IArrayFilterOperation"/>) operation or comparison against a string property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An AND combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation<string>> operation2, Action<OperationFilterOptions> options = default);

        /// <summary>Create a specification that ANDs two <see cref="IStringFilterOperation"/> operations or comparisons against a string
        /// property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An AND combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default);

        /// <summary>Create a specification that ANDs two <see cref="IBasicFilterOperation"/> (or <see cref="IArrayFilterOperation"/>) operations
        /// or comparisons against a string property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An AND combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions> options = default);

        #endregion

        #region OR

        /// <summary>Create a specification that ORs an <see cref="IBasicFilterOperation"/> (or <see cref="IArrayFilterOperation"/>)
        /// with an <see cref="IStringFilterOperation"/> operation or comparison against a string property on a
        /// <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An OR combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IBasicFilterOperation<string>> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions>? options = default);

        /// <summary>Create a specification that ORs an <see cref="IStringFilterOperation"/> with an <see cref="IBasicFilterOperation"/>
        /// (or <see cref="IArrayFilterOperation"/>) operation or comparison against a string property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An OR combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation<string>> operation2, Action<OperationFilterOptions>? options = default);

        /// <summary>Create a specification that ORs two <see cref="IStringFilterOperation"/> operations or comparisons against a string
        /// property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An OR combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions>? options = default);

        /// <summary>Create a specification that ORs two <see cref="IBasicFilterOperation"/> (or <see cref="IArrayFilterOperation"/>) operations
        /// or comparisons against a string property on a <typeparamref name="TType"/> instance.</summary>
        /// <param name="propertyExpression">An expression identifying the property to apply the filter operation against.</param>
        /// <param name="operation1">The left filter operation to be applied.</param>
        /// <param name="operation2">The right filter operation to be applied.</param>
        /// <param name="options">Optional options that control how the specification is constructed.</param>
        /// <returns>An OR combined specification that performs an operation or comparison against a specified string property on a
        /// <typeparamref name="TType"/> instance.</returns>
        ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions>? options = default);

        #endregion
    }
}