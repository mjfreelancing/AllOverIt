using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    public interface IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation);
        ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation);


        // There is no AND / OR version of this
        //ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IArrayOperation> operation);







        ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1, Func<TFilter, IStringFilterOperation> operation2);
        ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation1, Func<TFilter, IFilterOperation> operation2);






        ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1, Func<TFilter, IStringFilterOperation> operation2);
        ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation1, Func<TFilter, IFilterOperation> operation2);
    }
}