using AllOverIt.Expressions;
using AllOverIt.Extensions;
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
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory)
                : base(() => CreateResolver(propertyExpression, CreateValueExpression(value), predicateExpressionFactory))
        {
        }

        protected OperationBase(
            // The property building the predicate for
            Expression<Func<TEntity, TProperty>> propertyExpression,

            // The constant value used in the predicate
            IList<TProperty> values,

            // Creates the final expression
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory)
                : base(() => CreateResolver(propertyExpression, CreateValueExpression(values), predicateExpressionFactory))
        {
        }

        private static Expression<Func<TEntity, bool>> CreateResolver(Expression<Func<TEntity, TProperty>> propertyExpression, SystemExpression constant,
            Func<MemberExpression, SystemExpression, SystemExpression> predicateExpressionFactory)
        {
            var parameter = SystemExpression.Parameter(typeof(TEntity), "entity");


            // TODO: Add this is a utility
            // Get the full property chain
            MemberExpression member = null;
            var memberExpressions = propertyExpression.GetMemberExpressions();

            foreach (var memberExpression in memberExpressions)
            {
                var expression = (SystemExpression) member ?? parameter;
                member = SystemExpression.PropertyOrField(expression, memberExpression.Member.Name);
            }



            var predicate = predicateExpressionFactory.Invoke(member, constant);

            return SystemExpression.Lambda<Func<TEntity, bool>>(predicate, parameter);
        }

        private static SystemExpression CreateValueExpression<TValue>(TValue value)
        {
            // TODO: If not using parameterized values, simply return:
            // return SystemExpression.Constant(value);

            // Must use the runtime type, not the typeof(TValue) because IList<T> causes issues when the value is a List<T>
            return ExpressionUtils.CreateParameterizedValue(value, value.GetType());
        }
    }
}