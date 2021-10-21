using AllOverIt.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
    // Inspired by http://www.albahari.com/nutshell/predicatebuilder.aspx
    // Future reading material:
    //   https://github.com/scottksmith95/LINQKit
    //   http://tomasp.net/blog/linq-expand.aspx/
    //   http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx
    //   https://docs.microsoft.com/en-us/archive/blogs/meek/linq-to-entities-combining-predicates

    /// <summary>A helper class to build expression based predicates.</summary>
    public static class PredicateBuilder
    {
        /// <summary>Returns an expression that always evaluates to true.</summary>
        /// <typeparam name="TType">The source type of the lambda expression.</typeparam>
        /// <returns>An expression that always evaluates to true.</returns>
        public static Expression<Func<TType, bool>> True<TType>()
        {
            return _ => true;
        }

        /// <summary>Returns an expression that always evaluates to false.</summary>
        /// <typeparam name="TType">The source type of the lambda expression.</typeparam>
        /// <returns>An expression that always evaluates to false.</returns>
        public static Expression<Func<TType, bool>> False<TType>()
        {
            return _ => false;
        }

        /// <summary>Begins a chain of composed expressions that are logically OR'd or AND'd together, starting with
        /// a <see cref="True{TType}"/> or <see cref="False{TType}"/> expression.</summary>
        /// <typeparam name="TType">The source type of the lambda expression.</typeparam>
        /// <param name="initialState">The boolean value to be returned by the evaluated expression.</param>
        /// <returns>An expression that evaluates to the value of a provided initial state.</returns>
        /// <remarks>This method is normally used when there is a need to chain multiple Where clauses, starting
        /// with a value of false when OR'ing expressions, and true when AND'ing expressions.</remarks>
        public static Expression<Func<TType, bool>> Where<TType>(bool initialState)
        {
            return initialState
              ? True<TType>()
              : False<TType>();
        }

        /// <summary>Begins a chain of composed expressions that are logically OR'd or AND'd together.</summary>
        /// <typeparam name="TType">The source type of the lambda expression.</typeparam>
        /// <param name="expression">The first expression in the chain.</param>
        /// <returns>The same expression provided.</returns>
        public static Expression<Func<TType, bool>> Where<TType>(Expression<Func<TType, bool>> expression)
        {
            return expression;
        }

        /// <summary>Logically OR combines two expressions.</summary>
        /// <typeparam name="TType">The source type of the lambda expression.</typeparam>
        /// <param name="leftExpression">The expression to the left of the OR operation.</param>
        /// <param name="rightExpression">The expression to the right of the OR operation.</param>
        /// <returns>An expression that combines the two expressions as an OR operation.</returns>
        public static Expression<Func<TType, bool>> Or<TType>(this Expression<Func<TType, bool>> leftExpression,
            Expression<Func<TType, bool>> rightExpression)
        {
            _ = leftExpression.WhenNotNull(nameof(leftExpression));
            _ = rightExpression.WhenNotNull(nameof(rightExpression));

            return leftExpression.Compose(rightExpression, Expression.OrElse);
        }

        /// <summary>Logically AND combines two expressions.</summary>
        /// <typeparam name="TType">The source type of the lambda expression.</typeparam>
        /// <param name="leftExpression">The expression to the left of the AND operation.</param>
        /// <param name="expression2">The expression to the right of the AND operation.</param>
        /// <returns>An expression that combines the two expressions as an AND operation.</returns>
        public static Expression<Func<TType, bool>> And<TType>(this Expression<Func<TType, bool>> leftExpression,
            Expression<Func<TType, bool>> rightExpression)
        {
            _ = leftExpression.WhenNotNull(nameof(leftExpression));
            _ = rightExpression.WhenNotNull(nameof(rightExpression));

            return leftExpression.Compose(rightExpression, Expression.AndAlso);
        }

        /// <summary>Returns an expression that represents a bitwise complement of the provided expression.</summary>
        /// <typeparam name="TType">The source type of the lambda expression.</typeparam>
        /// <param name="expression">The source expression.</param>
        /// <returns>An expression that represents a bitwise complement of the provided expression.</returns>
        public static Expression<Func<TType, bool>> Not<TType>(this Expression<Func<TType, bool>> expression)
        {
            _ = expression.WhenNotNull(nameof(expression));

            var negated = Expression.Not(expression.Body);

            return Expression.Lambda<Func<TType, bool>>(negated, expression.Parameters);
        }

        private static Expression<TType> Compose<TType>(this Expression<TType> expression1, Expression<TType> expression2,
            Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of rightExpression to parameters of leftExpression)
            var map = expression1.Parameters
                .Select((first, index) => new
                {
                    First = first,
                    Second = expression2.Parameters[index]
                })
                .ToDictionary(parameter => parameter.Second, parameter => parameter.First);

            // replace parameters in the lambda rightExpression with parameters from leftExpression
            var secondBody = ParameterRebinder.ReplaceParameters(map, expression2.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<TType>(merge.Invoke(expression1.Body, secondBody), expression1.Parameters);
        }
    }
}