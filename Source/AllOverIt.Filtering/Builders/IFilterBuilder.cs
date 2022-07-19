using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    public interface IFilterBuilder<TType, TFilter>
       where TType : class
       where TFilter : class
    {
        // Gets the current state of the filter builder as a specification.
        ILinqSpecification<TType> AsSpecification { get; }

        // Gets the current logical expression to cater for additional chaining.
        ILogicalFilterBuilder<TType, TFilter> Current { get; }

        ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation);
        ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification);
    }
}