using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    public interface ILogicalFilterBuilder<TType, TFilter>
      where TType : class
      where TFilter : class, IFilter
    {
        ILogicalFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification);


        ILogicalFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification);
    }
}