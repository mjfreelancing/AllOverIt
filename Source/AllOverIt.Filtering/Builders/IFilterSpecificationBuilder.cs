using AllOverIt.Filtering.Filters;
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
        ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation);
        ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation);

        // Create a specification that ANDs two operations on a property.
        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1, Func<TFilter, IStringFilterOperation> operation2);
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation1, Func<TFilter, IFilterOperation> operation2);

        // Create a specification that OR two operations on a property.
        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1, Func<TFilter, IStringFilterOperation> operation2);
        ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation1, Func<TFilter, IFilterOperation> operation2);
    }
}