using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Operations;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    internal class FilterSpecificationBuilder<TType, TFilter> : IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        private readonly TFilter _filter;

        public FilterSpecificationBuilder(TFilter filter)
        {
            _filter = filter.WhenNotNull(nameof(filter));
        }



        public ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            return GetFilterSpecification(propertyExpression, operation);
        }

        // Caters for IOperation and IArrayOperation
        public ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IFilterOperation> operation)
        {
            return GetFilterSpecification(propertyExpression, operation);
        }



        #region AND Operations
        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1);
            var specification2 = GetFilterSpecification(propertyExpression, operation2);

            return specification1.And(specification2);
        }

        // Caters for IOperation and IArrayOperation
        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation1,
            Func<TFilter, IFilterOperation> operation2)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1);
            var specification2 = GetFilterSpecification(propertyExpression, operation2);

            return specification1.And(specification2);
        }
        #endregion



        #region OR Operations
        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1);
            var specification2 = GetFilterSpecification(propertyExpression, operation2);

            return specification1.Or(specification2);
        }

        // Caters for IOperation and IArrayOperation
        public ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IFilterOperation> operation1,
            Func<TFilter, IFilterOperation> operation2)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1);
            var specification2 = GetFilterSpecification(propertyExpression, operation2);

            return specification1.Or(specification2);
        }
        #endregion



        private ILinqSpecification<TType> GetFilterSpecification(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            var operand = operation.Invoke(_filter);

            return operand switch
            {
                IContains contains => new ContainsOperation<TType>(propertyExpression, contains.Value),
                INotContains notContains => new NotContainsOperation<TType>(propertyExpression, notContains.Value),
                IStartsWith startsWith => new StartsWithOperation<TType>(propertyExpression, startsWith.Value),
                IEndsWith endsWith => new EndsWithOperation<TType>(propertyExpression, endsWith.Value),
                _ => throw new InvalidOperationException("Unknown operation."),
            };
        }

        // Caters for IOperation and IArrayOperation
        private ILinqSpecification<TType> GetFilterSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IFilterOperation> operation)
        {
            var operand = operation.Invoke(_filter);

            return operand switch
            {
                IArrayFilterOperation array => GetFilterSpecification(propertyExpression, array),
                IEqualTo<TProperty> equalTo => new EqualToOperation<TType, TProperty>(propertyExpression, equalTo.Value),
                INotEqualTo<TProperty> equalTo => new NotEqualToOperation<TType, TProperty>(propertyExpression, equalTo.Value),
                IGreaterThan<TProperty> greaterThan => new GreaterThan<TType, TProperty>(propertyExpression, greaterThan.Value),
                IGreaterThanOrEqual<TProperty> greaterThanOrEqual => new GreaterThanOrEqual<TType, TProperty>(propertyExpression, greaterThanOrEqual.Value),
                ILessThan<TProperty> lessThan => new LessThan<TType, TProperty>(propertyExpression, lessThan.Value),
                ILessThanOrEqual<TProperty> lessThanOrEqual => new LessThanOrEqual<TType, TProperty>(propertyExpression, lessThanOrEqual.Value),
                _ => throw new InvalidOperationException($"Unknown operation {operand.GetType().GetFriendlyName()} for {propertyExpression}."),
            };
        }


        private ILinqSpecification<TType> GetFilterSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
           IArrayFilterOperation operation)
        {
            //var operand = operation.Invoke(_filter);

            return operation switch
            {
                IIn<TProperty> array => new In<TType, TProperty>(propertyExpression, array.Values),
                INotIn<TProperty> array => new NotIn<TType, TProperty>(propertyExpression, array.Values),
                _ => throw new InvalidOperationException("Unknown operation."),
            };
        }
    }
}