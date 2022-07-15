using AllOverIt.Assertion;
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

        public ILinqSpecification<TType> GetSpecification(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        public ILinqSpecification<TType> GetSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            return GetOperationSpecification(propertyExpression, operation);
        }

        #region AND Operations
        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);

            return specification1.And(specification2);
        }

        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);

            return specification1.And(specification2);
        }
        #endregion

        #region OR Operations
        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation1, Func<TFilter, IStringOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);

            return specification1.Or(specification2);
        }

        public ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation1, Func<TFilter, IOperation> operation2)
        {
            var specification1 = GetOperationSpecification(propertyExpression, operation1);
            var specification2 = GetOperationSpecification(propertyExpression, operation2);

            return specification1.Or(specification2);
        }
        #endregion

        private ILinqSpecification<TType> GetOperationSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var operand = operation.Invoke(_filter);

            if (operand is IGreaterThan<TProperty> greaterThan)
            {
                return new GreaterThan<TType, TProperty>(propertyExpression, greaterThan.Value);
            }
            else if (operand is ILessThan<TProperty> lessThan)
            {
                return new LessThan<TType, TProperty>(propertyExpression, lessThan.Value);
            }

            throw new InvalidOperationException("Unknown operation.");
        }

        private ILinqSpecification<TType> GetOperationSpecification(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var operand = operation.Invoke(_filter);

            if (operand is IContains contains)
            {
                return new Contains<TType>(propertyExpression, contains.Value);
            }
            else if (operand is IStartsWith startsWith)
            {
                return new StartsWith<TType>(propertyExpression, startsWith.Value);
            }

            throw new InvalidOperationException("Unknown operation.");
        }
    }
}