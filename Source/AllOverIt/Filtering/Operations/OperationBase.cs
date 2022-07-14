using AllOverIt.Extensions;
using AllOverIt.Patterns.Specification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SystemExpression = System.Linq.Expressions.Expression;    // avoid conflict with the Expression property on LinqSpecification

namespace AllOverIt.Filtering.Operations
{
    public abstract class OperationBase<TEntity, TProperty> : LinqSpecification<TEntity> where TEntity : class
    {
        protected OperationBase(
            // The property building the predicate for
            Expression<Func<TEntity, TProperty>> propertyExpression,

            // The constant value used in the predicate
            TProperty value,
            
            // Creates the final expression
            Func<MemberExpression, ConstantExpression, SystemExpression> predicateExpressionFactory)
                : base(() => CreateResolver(propertyExpression, SystemExpression.Constant(value), predicateExpressionFactory))
        {
        }

        protected OperationBase(
            // The property building the predicate for
            Expression<Func<TEntity, TProperty>> propertyExpression,

            // The constant value used in the predicate
            IList<TProperty> values,

            // Creates the final expression
            Func<MemberExpression, ConstantExpression, SystemExpression> predicateExpressionFactory)
                : base(() => CreateResolver(propertyExpression, SystemExpression.Constant(values), predicateExpressionFactory))
        {
        }

        private static Expression<Func<TEntity, bool>> CreateResolver(Expression<Func<TEntity, TProperty>> propertyExpression, ConstantExpression constant,
            Func<MemberExpression, ConstantExpression, SystemExpression> predicateExpressionFactory)
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



            var predicate = predicateExpressionFactory.Invoke(member as MemberExpression, constant);

            return SystemExpression.Lambda<Func<TEntity, bool>>(predicate, parameter);
        }
    }








    public sealed class In<TEntity, TProperty> : OperationBase<TEntity, TProperty> where TEntity : class
    {
        private static readonly MethodInfo ContainsMethod = typeof(IList).GetMethod("Contains", new[] { typeof(TProperty) });

        public In(Expression<Func<TEntity, TProperty>> propertyExpression, IList<TProperty> values)
            : base(propertyExpression, values, CreatePredicate)
        {
        }

        private static SystemExpression CreatePredicate(MemberExpression member, ConstantExpression constant)
        {
            return SystemExpression.Call(constant, ContainsMethod, member);
        }
    }

}