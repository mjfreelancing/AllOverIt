using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Filtering.Builders;
using AllOverIt.Patterns.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    internal abstract class OperationBase<TEntity, TProperty> : LinqSpecification<TEntity> where TEntity : class
    {
        protected OperationBase(
            // The property building the predicate for
            Expression<Func<TEntity, TProperty>> propertyExpression,

            // The constant value used in the predicate
            TProperty value,
            
            // Creates the final expression
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory,

            IFilterSpecificationBuilderOptions options)
                : base(() => CreateResolver(propertyExpression, CreateValueExpression(value, options.UseParameterizedQueries), predicateExpressionFactory))
        {
        }

        protected OperationBase(
            // The property building the predicate for
            Expression<Func<TEntity, TProperty>> propertyExpression,

            // The constant value used in the predicate
            IList<TProperty> values,

            // Creates the final expression
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory,

            IFilterSpecificationBuilderOptions options)
                : base(() => CreateResolver(propertyExpression, CreateValueExpression(values, options.UseParameterizedQueries), predicateExpressionFactory))
        {
        }

        private static Expression<Func<TEntity, bool>> CreateResolver(Expression<Func<TEntity, TProperty>> propertyExpression, SystemExpression constant,
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory)
        {
            var parameter = SystemExpression.Parameter(typeof(TEntity), "entity");

            var memberExpression = propertyExpression.GetPropertyOrFieldExpressionUsingParameter(parameter);

            var predicate = predicateExpressionFactory.Invoke(memberExpression, constant);

            return SystemExpression.Lambda<Func<TEntity, bool>>(predicate, parameter);
        }

        private static SystemExpression CreateValueExpression<TValue>(TValue value, bool useParameterizedQueries)
        {
            return useParameterizedQueries
                ? ExpressionUtils.CreateParameterizedValue(value, value.GetType())
                : SystemExpression.Constant(value);
        }
    }
}