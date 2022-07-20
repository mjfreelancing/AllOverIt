using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    public interface IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class
    {
        // Create a specification for a single operation against a property.
        ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        // Also handles IArrayFilterOperation
        ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation,
            Action<OperationFilterOptions> options = default);

        // Create a specification that ANDs two operations on a property.
        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default);

        // Also handles IArrayFilterOperation
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions> options = default);

        // Create a specification that OR two operations on a property.
        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default);

        // Also handles IArrayFilterOperation
        ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions> options = default);
    }
}