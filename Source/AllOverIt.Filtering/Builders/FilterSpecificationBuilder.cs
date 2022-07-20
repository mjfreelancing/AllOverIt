using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Operations;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    internal class FilterSpecificationBuilder<TType, TFilter> : IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class
    {
        private readonly IReadOnlyDictionary<Type, Type> _filterOperations;
        private readonly TFilter _filter;
        private readonly IFilterSpecificationBuilderOptions _options;

        public static readonly ILinqSpecification<TType> SpecificationIgnore = LinqSpecification<TType>.Create(_ => true);

        public FilterSpecificationBuilder(TFilter filter, IFilterSpecificationBuilderOptions options)
        {
            _filter = filter.WhenNotNull(nameof(filter));
            _options = options.WhenNotNull(nameof(options));


            var ops = new Dictionary<Type, Type>
            {
                // IArrayFilterOperation
                { typeof(IIn<>), typeof(InOperation<,>) },
                { typeof(INotIn<>), typeof(NotInOperation<,>) },

                // IBasicFilterOperation
                { typeof(IEqualTo<>), typeof(EqualToOperation<,>) },
                { typeof(INotEqualTo<>), typeof(NotEqualToOperation<,>) },
                { typeof(IGreaterThan<>), typeof(GreaterThanOperation<,>) },
                { typeof(IGreaterThanOrEqual<>), typeof(GreaterThanOrEqualOperation<,>) },
                { typeof(ILessThan<>), typeof(LessThanOperation<,>) },
                { typeof(ILessThanOrEqual<>), typeof(LessThanOrEqualOperation<,>) }
            };

            _filterOperations = ops;
        }

        public ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            return GetFilterSpecification(propertyExpression, operation);
        }

        // Caters for IOperation and IArrayOperation
        public ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation)
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
        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2)
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
        public ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2)
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

            Throw<InvalidOperationException>.WhenNull($"The filter operation resolver on {propertyExpression} cannot return null.");

            if (_options.IgnoreNullFilterValues)
            {
                switch (operation)
                {
                    case IStringFilterOperation op1 when op1.Value is null:
                        return SpecificationIgnore;

                    default:    // fall through, continue processing                        
                        break;
                }
            }

            try
            {
                return operation switch
                {
                    IContains contains => new ContainsOperation<TType>(propertyExpression, contains.Value, _options),
                    INotContains notContains => new NotContainsOperation<TType>(propertyExpression, notContains.Value, _options),
                    IStartsWith startsWith => new StartsWithOperation<TType>(propertyExpression, startsWith.Value, _options),
                    IEndsWith endsWith => new EndsWithOperation<TType>(propertyExpression, endsWith.Value, _options),

                    _ => throw new InvalidOperationException($"Cannot apply {operation.GetType().GetFriendlyName()} to {propertyExpression}."),
                };
            }
            catch (NullNotSupportedException)
            {
                throw new NullNotSupportedException($"The filter operation on {propertyExpression} does not support null values.");
            }
        }

        private ILinqSpecification<TType> GetFilterSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> filterOperation)
        {
            var operation = filterOperation.Invoke(_filter);

            Throw<InvalidOperationException>.WhenNull($"The filter operation resolver on {propertyExpression} cannot return null.");

            // The TProperty is based on the entity's property type. When this is non-nullable but the filter's value type is
            // nullable we need to use reflection to determine if the filter's value is null - we cannot cast the operation to
            // something like IBasicFilterOperation<TProperty?>.
            var operationType = operation.GetType();

            if (_options.IgnoreNullFilterValues)
            {
                // Nullable<T>
                var operationTypeIsNullable = operationType.GetGenericArguments()[0].IsNullableType();

                if (operationTypeIsNullable)
                {
                    var propInfo = operationType.GetProperty(nameof(IFilterOperationType<TProperty>.Value));
                    var value = propInfo.GetValue(operation);

                    if (value is null)
                    {
                        return SpecificationIgnore;
                    }
                }
            }

            var operationKey = _filterOperations.Keys.SingleOrDefault(type => operationType.IsDerivedFrom(type));

            Throw<InvalidOperationException>.WhenNull(operationKey, $"The operation type '{operationType.GetFriendlyName()}' is not registered with a specification factory.");

            var specificationOperationType = _filterOperations[operationKey];

            try
            {
                return CreateSpecificationOperation(specificationOperationType, operation, propertyExpression);
            }
            catch (NullNotSupportedException)
            {
                throw new NullNotSupportedException($"The filter operation on {propertyExpression} does not support null values.");
            }
        }

        // As an example, creates a EqualToOperation<,> based on a IEqualTo<>
        // Caters for IBasicFilterOperation and IArrayFilterOperation
        private ILinqSpecification<TType> CreateSpecificationOperation<TProperty>(Type specificationOperationType, IBasicFilterOperation operation, Expression<Func<TType, TProperty>> propertyExpression)
        {
            // operation, such as IEqualTo<>
            var operationType = operation.GetType();

            // TProperty may be nullable
            var typeArgs = new[] { typeof(TType), typeof(TProperty) };

            // specificationOperationType, such as EqualToOperation<,>
            var genericOperation = specificationOperationType.MakeGenericType(typeArgs);

            // TODO: Assumes not IArrayFilterOperation  - needs to be updated
            var value = operationType.GetProperty(nameof(IFilterOperationType<TProperty>.Value)).GetValue(operation);

            return (ILinqSpecification<TType>) Activator.CreateInstance(genericOperation, new object[] { propertyExpression, value, _options });
        }
    }
}