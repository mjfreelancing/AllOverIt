using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Filtering.Builders
{
    internal class FilterSpecificationBuilder<TType, TFilter> : IFilterSpecificationBuilder<TType, TFilter>
        where TType : class
        where TFilter : class
    {
        private readonly IReadOnlyDictionary<Type, Type> _filterOperations = new Dictionary<Type, Type>
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

        private readonly TFilter _filter;
        private readonly IQueryFilterOptions _options;

        public static readonly ILinqSpecification<TType> SpecificationIgnore = LinqSpecification<TType>.Create(_ => true);

        public FilterSpecificationBuilder(TFilter filter, IQueryFilterOptions options)
        {
            _filter = filter.WhenNotNull(nameof(filter));
            _options = options.WhenNotNull(nameof(options));
        }

        public ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions> options)
        {
            return GetFilterSpecification(propertyExpression, operation, options);
        }

        // Caters for IOperation and IArrayOperation
        public ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation, Action<OperationFilterOptions> options)
        {
            return GetFilterSpecification(propertyExpression, operation, options);
        }

        #region AND Operations
        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return specification1.And(specification2);
        }

        // Caters for IOperation and IArrayOperation
        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions> options)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return specification1.And(specification2);
        }
        #endregion

        #region OR Operations
        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return specification1.Or(specification2);
        }

        // Caters for IOperation and IArrayOperation
        public ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions> options)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return specification1.Or(specification2);
        }
        #endregion

        private OperationFilterOptions GetOperationFilterOptions(Action<OperationFilterOptions> action)
        {
            var filterOptions = new OperationFilterOptions
            {
                UseParameterizedQueries = _options.UseParameterizedQueries,
                StringComparison = _options.StringComparison,
                IgnoreNullFilterValue = _options.IgnoreNullFilterValues
            };

            action?.Invoke(filterOptions);

            return filterOptions;
        }

        private ILinqSpecification<TType> GetFilterSpecification(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> filterOperation,
            Action<OperationFilterOptions> options)
        {
            var operation = filterOperation.Invoke(_filter);
            var filterOptions = GetOperationFilterOptions(options);

            Throw<InvalidOperationException>.WhenNull($"The filter operation resolver on {propertyExpression} cannot return null.");

            if (filterOptions.IgnoreNullFilterValue)
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
                    IContains contains => new ContainsOperation<TType>(propertyExpression, contains.Value, filterOptions),
                    INotContains notContains => new NotContainsOperation<TType>(propertyExpression, notContains.Value, filterOptions),
                    IStartsWith startsWith => new StartsWithOperation<TType>(propertyExpression, startsWith.Value, filterOptions),
                    IEndsWith endsWith => new EndsWithOperation<TType>(propertyExpression, endsWith.Value, filterOptions),

                    _ => throw new InvalidOperationException($"Cannot apply {operation.GetType().GetFriendlyName()} to {propertyExpression}."),
                };
            }
            catch (NullNotSupportedException)
            {
                throw new NullNotSupportedException($"The filter operation on {propertyExpression} does not support null values.");
            }
        }

        private ILinqSpecification<TType> GetFilterSpecification<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> filterOperation, Action<OperationFilterOptions> options)
        {
            var operation = filterOperation.Invoke(_filter);
            var filterOptions = GetOperationFilterOptions(options);

            Throw<InvalidOperationException>.WhenNull($"The filter operation resolver on {propertyExpression} cannot return null.");

            // The TProperty is based on the entity's property type. When this is non-nullable but the filter's value type is
            // nullable we need to use reflection to determine if the filter's value is null - we cannot cast the operation to
            // something like IBasicFilterOperation<TProperty?>.
            var operationType = operation.GetType();

            if (filterOptions.IgnoreNullFilterValue)
            {
                // Nullable<T>
                var operationTypeIsNullable = operationType.GetGenericArguments()[0].IsNullableType();
                var operationIsArray = operationType.IsDerivedFrom(typeof(IArrayFilterOperation));

                if (operationTypeIsNullable || operationIsArray)
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
                return CreateSpecificationOperation(specificationOperationType, operation, propertyExpression, filterOptions);
            }
            catch (NullNotSupportedException)
            {
                throw new NullNotSupportedException($"The filter operation {operationType.GetFriendlyName()}() on {propertyExpression} does not support null values.");
            }
        }

        // As an example, creates an EqualToOperation<,> based on a IEqualTo<>
        // Caters for IBasicFilterOperation and IArrayFilterOperation
        private static ILinqSpecification<TType> CreateSpecificationOperation<TProperty>(Type specificationOperationType, IBasicFilterOperation operation,
            Expression<Func<TType, TProperty>> propertyExpression, IOperationFilterOptions options)
        {
            // operation, such as IEqualTo<>
            var operationType = operation.GetType();

            // TProperty may be nullable
            var typeArgs = new[] { typeof(TType), typeof(TProperty) };

            // specificationOperationType, such as EqualToOperation<,>
            var genericOperation = specificationOperationType.MakeGenericType(typeArgs);

            // Caters for IFilterOperationType<TProperty> and IArrayFilterOperation<TProperty>
            var value = operationType.GetProperty(nameof(IFilterOperationType<TProperty>.Value)).GetValue(operation);

            try
            {
                return operation is IArrayFilterOperation
                    ? CreateArraySpecificationOperation(genericOperation, propertyExpression, value, options)
                    : CreateBasicSpecificationOperation(genericOperation, propertyExpression, value, options);
            }
            catch (TargetInvocationException exception)
            {
                // The operation may throw a NullNotSupportedException
                throw exception.InnerException ?? exception;
            }
        }


        private static ILinqSpecification<TType> CreateBasicSpecificationOperation<TProperty>(Type genericOperation,
            Expression<Func<TType, TProperty>> propertyExpression, object value, IOperationFilterOptions options)
        {
            // No special consideration is required when the value is double (for example) and TProperty is double?
            var ctor = genericOperation.GetConstructor(new[] {
                        typeof(Expression<Func<TType, TProperty>>),
                        typeof(TProperty),
                        typeof(IOperationFilterOptions)
                    });

            return (ILinqSpecification<TType>) ctor.Invoke(new object[] { propertyExpression, value, options });
        }

        private static ILinqSpecification<TType> CreateArraySpecificationOperation<TProperty>(Type genericOperation,
            Expression<Func<TType, TProperty>> propertyExpression, object values, IOperationFilterOptions options)
        {
            Throw<InvalidOperationException>.When(values is not IList, "Array based specifications expected an IList<T>.");

            // The array based operations require special consideration when the value is double (for example) and
            // TProperty is double? because an error occurs due to List<double> cannot be converted to IList<double?>.

            if (values.GetType().GetGenericArguments()[0] != typeof(TProperty))
            {
                values = ConvertListElements((IList) values, typeof(TProperty));
            }

            var ctor = genericOperation.GetConstructor(new[]
            {
                typeof(Expression<Func<TType, TProperty>>),
                typeof(IList<TProperty>),
                typeof(IOperationFilterOptions)
            });

            return (ILinqSpecification<TType>) ctor.Invoke(new object[] { propertyExpression, values, options });
        }

        private static IList ConvertListElements(IList elements, Type elementType)
        {
            var listType = typeof(List<>).MakeGenericType(new[] { elementType });

            var typedList = (IList) Activator.CreateInstance(listType);

            foreach (var element in elements)
            {
                typedList.Add(element);
            }

            return typedList;
        }
    }
}