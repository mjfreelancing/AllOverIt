using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Extensions;
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
        where TFilter : class
    {
        private readonly TFilter _filter;
        private readonly IFilterSpecificationBuilderOptions _options;

        public FilterSpecificationBuilder(TFilter filter, IFilterSpecificationBuilderOptions options)
        {
            _filter = filter.WhenNotNull(nameof(filter));
            _options = options.WhenNotNull(nameof(options));
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
            Func<TFilter, IStringFilterOperation> filterOperation)
        {
            var operation = filterOperation.Invoke(_filter);

            return operation switch
            {
                IContains contains => new ContainsOperation<TType>(propertyExpression, contains.Value, _options),
                INotContains notContains => new NotContainsOperation<TType>(propertyExpression, notContains.Value, _options),
                IStartsWith startsWith => new StartsWithOperation<TType>(propertyExpression, startsWith.Value, _options),
                IEndsWith endsWith => new EndsWithOperation<TType>(propertyExpression, endsWith.Value, _options),

                _ => throw new InvalidOperationException($"Cannot apply {operation.GetType().GetFriendlyName()} to {propertyExpression}."),
            };
        }

        private ILinqSpecification<TType> GetFilterSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IFilterOperation> filterOperation)
        {
            var operation = filterOperation.Invoke(_filter);

            return operation switch
            {
                // IArrayFilterOperation
                IIn<TProperty> array => new InOperation<TType, TProperty>(propertyExpression, array.Values, _options),
                INotIn<TProperty> array => new NotInOperation<TType, TProperty>(propertyExpression, array.Values, _options),

                // IFilterOperation
                IEqualTo<TProperty> equalTo => new EqualToOperation<TType, TProperty>(propertyExpression, equalTo.Value, _options),
                INotEqualTo<TProperty> equalTo => new NotEqualToOperation<TType, TProperty>(propertyExpression, equalTo.Value, _options),
                IGreaterThan<TProperty> greaterThan => new GreaterThanOperation<TType, TProperty>(propertyExpression, greaterThan.Value, _options),
                IGreaterThanOrEqual<TProperty> greaterThanOrEqual => new GreaterThanOrEqualOperation<TType, TProperty>(propertyExpression, greaterThanOrEqual.Value, _options),
                ILessThan<TProperty> lessThan => new LessThanOperation<TType, TProperty>(propertyExpression, lessThan.Value, _options),
                ILessThanOrEqual<TProperty> lessThanOrEqual => new LessThanOrEqualOperation<TType, TProperty>(propertyExpression, lessThanOrEqual.Value, _options),

                _ => throw new InvalidOperationException($"Cannot apply {operation.GetType().GetFriendlyName()} to {propertyExpression}."),
            };
        }
    }
}