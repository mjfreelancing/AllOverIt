using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    public interface IFilterBuilder<TType, TFilter>
       where TType : class
       where TFilter : class, IFilter
    {
        ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification);
    }
}