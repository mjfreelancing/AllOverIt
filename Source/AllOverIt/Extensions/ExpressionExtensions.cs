using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Expression"/> types.</summary>
    public static class ExpressionExtensions
    {
        /// <summary>Recursively get all <see cref="MemberExpression"/> expressions linked to <paramref name="expression"/>.</summary>
        /// <param name="expression">The expression to get all linked <see cref="MemberExpression"/> expressions.</param>
        /// <returns>All linked <see cref="MemberExpression"/> expressions, including <paramref name="expression"/>.</returns>
        public static IEnumerable<MemberExpression> GetMemberExpressions(this Expression expression)
        {
            _ = expression.WhenNotNull(nameof(expression));

            var memberExpression = UnwrapMemberExpression(expression);

            IEnumerable<MemberExpression> GetMembers()
            {
                while (memberExpression != null)
                {
                    yield return memberExpression;

                    memberExpression = memberExpression.Expression.UnwrapMemberExpression();
                }
            }

            return GetMembers().Reverse();
        }

        /// <summary>Constructs a <see cref="MemberExpression"/> from a property or field accessor expression and a <see cref="ParameterExpression"/>.</summary>
        /// <typeparam name="TType">The object type containing the property.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyOrFieldExpression">The property or field accessor expression.</param>
        /// <param name="parameter">The parameter to use when constructing the <see cref="MemberExpression"/>.</param>
        /// <returns>A <see cref="MemberExpression"/> representing the property or field accessor expression. This expression can later be used
        /// to obtain the value of the property or field, or convert it to a <see cref="ConstantExpression"/>./returns>
        public static MemberExpression GetPropertyOrFieldExpressionUsingParameter<TType, TProperty>(
            this Expression<Func<TType, TProperty>> propertyOrFieldExpression, ParameterExpression parameterExpression)
        {
            _ = propertyOrFieldExpression.WhenNotNull(nameof(propertyOrFieldExpression));
            _ = parameterExpression.WhenNotNull(nameof(parameterExpression));

            MemberExpression member = null;
            var memberExpressions = propertyOrFieldExpression.GetMemberExpressions();

            foreach (var memberExpression in memberExpressions)
            {
                var expression = (Expression) member ?? parameterExpression;
                member = Expression.PropertyOrField(expression, memberExpression.Member.Name);
            }

            return member;
        }

        /// <summary>Constructs a <see cref="MemberExpression"/> from a property or field accessor expression. The expression will be
        /// constructed from a <see cref="ParameterExpression"/>.</summary>
        /// <typeparam name="TType">The object type containing the property.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyOrFieldExpression">The property or field accessor expression.</param>
        /// <returns>A <see cref="MemberExpression"/> representing the property or field accessor expression. This expression can later be used
        /// to obtain the value of the property or field, or convert it to a <see cref="ConstantExpression"/>./returns>
        public static MemberExpression GetParameterPropertyOrFieldExpression<TType, TProperty>(this Expression<Func<TType, TProperty>> propertyOrFieldExpression)
        {
            _ = propertyOrFieldExpression.WhenNotNull(nameof(propertyOrFieldExpression));

            var parameter = Expression.Parameter(typeof(TType), "entity");

            return GetPropertyOrFieldExpressionUsingParameter(propertyOrFieldExpression, parameter);
        }

        /// <summary>Gets the <paramref name="expression"/> as a <see cref="MemberExpression"/>.</summary>
        /// <param name="expression">The expression to be unwrapped as a <see cref="MemberExpression"/>.</param>
        /// <returns>
        /// If <paramref name="expression"/> is a <see cref="MemberExpression"/> then the same expression is returned.
        /// If <paramref name="expression"/> is a <see cref="LambdaExpression"/> then its Body is returned if it is a
        /// <see cref="MemberExpression"/>, or a <see cref="UnaryExpression"/> who's Operand is a <see cref="MemberExpression"/>.
        /// In all other cases, null is returned.</returns>
        public static MemberExpression UnwrapMemberExpression(this Expression expression)
        {
            _ = expression.WhenNotNull(nameof(expression));

            return expression switch
            {
                MemberExpression memberExpression => memberExpression,
                LambdaExpression {Body: MemberExpression memberExpression} => memberExpression,
                LambdaExpression {Body: UnaryExpression unaryExpression} => (unaryExpression.Operand as MemberExpression),
                _ => null
            };
        }

        /// <summary>Gets the property or field member of a <see cref="MemberExpression"/>, unwrapped from a <see cref="LambdaExpression"/> if required.</summary>
        /// <param name="expression">The expression containing the field or property member.</param>
        /// <returns>The property or field member.</returns>
        public static MemberInfo GetPropertyOrFieldMemberInfo(this Expression expression)
        {
            _ = expression.WhenNotNull(nameof(expression));

            var memberExpression = expression.UnwrapMemberExpression();

            _ = memberExpression ?? throw new InvalidOperationException("Expected a property or field access expression.");

            return memberExpression.Member;
        }

        /// <summary>
        /// Gets the value of an expression. Supports constants (<see cref="ConstantExpression"/>), fields and properties (<see cref="MemberExpression"/>),
        /// method calls <see cref="MethodCallExpression"/>, and other expressions that require dynamic invocation. <see cref="LambdaExpression"/>
        /// expressions are also supported if its body can be evaluated.
        /// </summary>
        /// <param name="expression">The expression to be evaluated.</param>
        /// <returns>The value of the <paramref name="expression"/>.</returns>
        public static object GetValue(this Expression expression)
        {
            try
            {
                static object EvalMemberExpression(MemberExpression memberExpression)
                {
                    var value = GetValue(memberExpression.Expression);

                    return memberExpression.Member.GetValue(value);
                }

                static object EvalMethodCallExpression(MethodCallExpression methodCallExpression)
                {
                    var obj = methodCallExpression.Object.GetValue();
                    var parameters = methodCallExpression.Arguments.Select(e => e.GetValue()).ToArray();

                    return methodCallExpression.Method.Invoke(obj, parameters);
                }

                static object EvalDynamicInvocationResult(Expression expression)
                {
                    var lambdaExpression = Expression.Lambda(expression);
                    var func = lambdaExpression.Compile();

                    return func.DynamicInvoke();
                }

                return expression switch
                {
                    null => null,

                    ConstantExpression constantExpression => constantExpression.Value,

                    MemberExpression memberExpression => EvalMemberExpression(memberExpression),

                    MethodCallExpression methodCallExpression => EvalMethodCallExpression(methodCallExpression),

                    LambdaExpression lambdaExpression => GetValue(lambdaExpression.Body),

                    ParameterExpression _ => throw new ArgumentOutOfRangeException(nameof(expression), $"A {nameof(ParameterExpression)} does not have a value"),

                    _ => EvalDynamicInvocationResult(expression)
                };
            }
            catch (TargetInvocationException exception)
            {
                // The InnerException will never be null - it holds the underlying exception thrown by the invoked method
                throw exception.InnerException;
            }
        }

        /// <summary>Casts or type converts an <see cref="Expression"/> to a specified target type.</summary>
        /// <param name="expression">The expression to be cast or converted</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>If the <paramref name="targetType"/> is assignable from the <paramref name="expression"/>'s type then the same reference is returned.
        /// If the target type is a non-nullable value type then a type conversion is performed, otherwise an explicit reference or boxing conversion is
        /// performed.</returns>
        public static Expression CastOrConvertTo(this Expression expression, Type targetType)
        {
            if (targetType.IsAssignableFrom(expression.Type))
            {
                return expression;
            }

            return targetType.IsValueType && !targetType.IsNullableType()
                ? Expression.Convert(expression, targetType)            // Type conversion
                : Expression.TypeAs(expression, targetType);            // Explicit reference or boxing conversion
        }
    }
}