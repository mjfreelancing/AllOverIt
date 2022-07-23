﻿using AllOverIt.Assertion;
using AllOverIt.Caching;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using AllOverIt.Reflection;
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
        private static readonly GenericCache _operationTypePropertyGetters = new();

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

        // Return True so a single 'ignored' expression behaves as if it didn't exist
        public static readonly ILinqSpecification<TType> SpecificationIgnore = LinqSpecification<TType>.Create(_ => true);

        // Note: IBasicFilterOperation also caters for IArrayFilterOperation

        public FilterSpecificationBuilder(TFilter filter, IQueryFilterOptions options)
        {
            _filter = filter.WhenNotNull(nameof(filter));
            _options = options.WhenNotNull(nameof(options));
        }

        #region Create

        public ILinqSpecification<TType> Create(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation,
            Action<OperationFilterOptions> options = default)
        {
            _ = propertyExpression.WhenNotNull(nameof(propertyExpression));
            _ = operation.WhenNotNull(nameof(operation));

            return GetFilterSpecification(propertyExpression, operation, options);
        }

        public ILinqSpecification<TType> Create<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation, Action<OperationFilterOptions> options = default)
        {
            _ = propertyExpression.WhenNotNull(nameof(propertyExpression));
            _ = operation.WhenNotNull(nameof(operation));

            return GetFilterSpecification(propertyExpression, operation, options);
        }

        #endregion

        #region AND

        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IBasicFilterOperation<string>> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.And);
        }

        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation<string>> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.And);
        }

        public ILinqSpecification<TType> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.And);
        }

        public ILinqSpecification<TType> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.And);
        }

        #endregion

        #region OR

        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IBasicFilterOperation<string>> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.Or);
        }

        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation<string>> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.Or);
        }

        public ILinqSpecification<TType> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringFilterOperation> operation1,
            Func<TFilter, IStringFilterOperation> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.Or);
        }

        public ILinqSpecification<TType> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IBasicFilterOperation> operation1,
            Func<TFilter, IBasicFilterOperation> operation2, Action<OperationFilterOptions> options = default)
        {
            var specification1 = GetFilterSpecification(propertyExpression, operation1, options);
            var specification2 = GetFilterSpecification(propertyExpression, operation2, options);

            return CombineSpecifications(specification1, specification2, LinqSpecificationExtensions.Or);
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
                var genericArgumentType = operationType.GetGenericArguments()[0];

                // Looking for typeof(string) and not IStringFilterOperation because other non (explicit) string operations support strings
                var argTypeIsNullable = genericArgumentType == typeof(string) || genericArgumentType.IsNullableType();

                var operationIsArray = operationType.IsDerivedFrom(typeof(IArrayFilterOperation));

                if (argTypeIsNullable || operationIsArray)
                {
                    var value = GetOperationValue<TProperty>(operationType, operation);

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

        // TODO: Check for opportunities to improve performance related to reflection
        //
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
            var value = GetOperationValue<TProperty>(operationType, operation);

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

        private ILinqSpecification<TType> CombineSpecifications(ILinqSpecification<TType> specification1, ILinqSpecification<TType> specification2,
            Func<ILinqSpecification<TType>, ILinqSpecification<TType>, ILinqSpecification<TType>> action)
        {
            // simplify the expression if either specifications are to be ignored
            if (specification1 != SpecificationIgnore && specification2 != SpecificationIgnore)
            {
                return action.Invoke(specification1, specification2);
            }

            return specification1 == SpecificationIgnore
                ? specification2
                : specification1;
        }

        private static object GetOperationValue<TProperty>(Type operationType, object operation)
        {
            var key = new GenericCacheKey<Type>(operationType);

            var propertyGetter = _operationTypePropertyGetters.GetOrAdd(key, cacheKey =>
            {
                var opType = ((GenericCacheKey<Type>) cacheKey).Key1;
                var propInfo = opType.GetProperty(nameof(IFilterOperationType<TProperty>.Value));

                return PropertyHelper.CreatePropertyGetter(propInfo);
            });


            return propertyGetter.Invoke(operation);

            // For reference, the non-cached approach is:
            //
            // return operationType
            //     .GetProperty(nameof(IFilterOperationType<TProperty>.Value))
            //     .GetValue(operation);
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