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
        // Gets the current state of the filter builder as a specification.
        ILinqSpecification<TType> AsSpecification { get; }

        // Gets the current logical expression to cater for additional chaining.
        ILogicalFilterBuilder<TType, TFilter> Current { get; }

        ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        // Also handles IArrayFilterOperation
        ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification);
    }
}