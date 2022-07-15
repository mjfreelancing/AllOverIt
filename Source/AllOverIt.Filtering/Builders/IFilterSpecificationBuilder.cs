using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    public interface IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        ILinqSpecification<TType> GetSpecification(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILinqSpecification<TType> GetSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);

        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2);

        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2);
        ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2);
    }
}