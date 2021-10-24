using AllOverIt.Reflection;
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
        public static IEnumerable<MemberExpression> GetMemberExpressions(this MemberExpression expression)
        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));

            IEnumerable<MemberExpression> GetMembers()
            {
                while (expression != null)
                {
                    yield return expression;

                    //expression = expression.Expression as MemberExpression;
                    expression = expression.Expression.UnwrapMemberExpression();
                }
            }

            return GetMembers().Reverse();
        }

        /// <summary>Gets the <paramref name="expression"/> as a <see cref="MemberExpression"/>.</summary>
        /// <param name="expression">The expression to be unwrapped as a <see cref="MemberExpression"/>.</param>
        /// <returns>
        /// If <paramref name="expression"/> is a <see cref="MemberExpression"/> then the same expression is returned.
        /// If <paramref name="expression"/> is a <see cref="LambdaExpression"/> then its Body is returned if it is a
        /// <see cref="MemberExpression"/>, or a <see cref="UnaryExpression"/> whos' Operand is a <see cref="MemberExpression"/>.
        /// In all other cases, null is returned.</returns>
        public static MemberExpression UnwrapMemberExpression(this Expression expression)
        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));

            return expression switch
            {
                MemberExpression memberExpression => memberExpression,
                LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression => memberExpression,
                LambdaExpression lambdaExpression when lambdaExpression.Body is UnaryExpression unaryExpression => (unaryExpression.Operand as MemberExpression),
                _ => null
            };
        }

        /// <summary>
        /// Gets the field or property member of a <see cref="MemberExpression"/>, unwrapped from a <see cref="LambdaExpression"/> if required.
        /// </summary>
        /// <param name="expression">The expression containing the field or property member.</param>
        /// <returns>The field or property member.</returns>
        public static MemberInfo GetFieldOrProperty(this Expression expression)
        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.UnwrapMemberExpression();

            _ = memberExpression ?? throw new InvalidOperationException("Expected a property or field access expression");

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

                    var member = memberExpression.Member;

                    return ReflectionHelper.GetMemberValue(member, value);
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
                // ReSharper disable once PossibleNullReferenceException
                throw exception.InnerException;
            }
        }
    }
}