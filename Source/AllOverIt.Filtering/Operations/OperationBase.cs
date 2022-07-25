using AllOverIt.Assertion;
using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal abstract class OperationBase<TEntity, TProperty> : LinqSpecification<TEntity> where TEntity : class
    {
        protected static readonly bool PropertyIsString = typeof(TProperty) == typeof(string);

        protected OperationBase(
            // The property building the predicate for
            Expression<Func<TEntity, TProperty>> propertyExpression,

            // The constant value used in the predicate
            TProperty value,
            
            // Indicates if an exception is to be thrown when the provided value is null
            bool supportsNull,

            // Creates the final expression
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory,

            IOperationFilterOptions options)
                : base(() => CreateResolver(propertyExpression, CreateValueExpression(value, supportsNull, options), predicateExpressionFactory))
        {
        }

        protected OperationBase(
            // The property building the predicate for
            Expression<Func<TEntity, TProperty>> propertyExpression,

            // The constant value used in the predicate
            IList<TProperty> values,

            // Creates the final expression
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory,

            IOperationFilterOptions options)
                : base(() => CreateResolver(propertyExpression, CreateValueExpression(values, false, options), predicateExpressionFactory))
        {
        }

        private static Expression<Func<TEntity, bool>> CreateResolver(Expression<Func<TEntity, TProperty>> propertyExpression, SystemExpression constant,
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory)
        {
            _ = propertyExpression.WhenNotNull(nameof(propertyExpression));

            var parameter = SystemExpression.Parameter(typeof(TEntity), "entity");

            var memberExpression = propertyExpression.GetPropertyOrFieldExpressionUsingParameter(parameter);

            var predicate = predicateExpressionFactory.Invoke(memberExpression, constant);

            return SystemExpression.Lambda<Func<TEntity, bool>>(predicate, parameter);
        }

        private static SystemExpression CreateValueExpression<TValue>(TValue value, bool supportsNull, IOperationFilterOptions options)
        {
            _ = options.WhenNotNull(nameof(options));

            // Need to deal with null strings and nullables. Could use Nullable.GetUnderlyingType() for nullable
            // types but the approach below works for both cases.

            // eg bool? on the entity
            var expectedType = typeof(TValue);

            // eg bool value provided for comparison
            var valueType = value == null
                ? typeof(TValue)
                : value.GetType();

            if (valueType != expectedType)
            {
                valueType = expectedType;
            }

            var result = options.UseParameterizedQueries
                ? ExpressionUtils.CreateParameterizedValue(value, valueType)
                : SystemExpression.Constant(value, valueType);

            if (!supportsNull && result.GetValue() == null)
            {
                throw new NullNotSupportedException();
            }

            return result;
        }
    }
}