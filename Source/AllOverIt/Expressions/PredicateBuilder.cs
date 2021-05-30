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

    public static class PredicateBuilder
    {
        public static Expression<Func<TType, bool>> True<TType>()
        {
            return f => true;
        }

        public static Expression<Func<TType, bool>> False<TType>()
        {
            return f => false;
        }

        public static Expression<Func<TType, bool>> Where<TType>(bool initialState)
        {
            return initialState
              ? True<TType>()
              : False<TType>();
        }

        public static Expression<Func<TType, bool>> Where<TType>(Expression<Func<TType, bool>> expression)
        {
            return expression;
        }

        public static Expression<Func<TType, bool>> Or<TType>(this Expression<Func<TType, bool>> expression1, Expression<Func<TType, bool>> expression2)
        {
            _ = expression1 ?? throw new ArgumentNullException(nameof(expression1));
            _ = expression2 ?? throw new ArgumentNullException(nameof(expression2));

            return expression1.Compose(expression2, Expression.OrElse);

            //var invokedExpr = Expression.Invoke(expression2, expression1.Parameters);
            //return Expression.Lambda<Func<TType, bool>>(Expression.OrElse(expression1.Body, invokedExpr), expression1.Parameters);
        }

        public static Expression<Func<TType, bool>> And<TType>(this Expression<Func<TType, bool>> expression1, Expression<Func<TType, bool>> expression2)
        {
            _ = expression1 ?? throw new ArgumentNullException(nameof(expression1));
            _ = expression2 ?? throw new ArgumentNullException(nameof(expression2));

            return expression1.Compose(expression2, Expression.AndAlso);

            //var invokedExpr = Expression.Invoke(expression2, expression1.Parameters);
            //return Expression.Lambda<Func<TType, bool>>(Expression.AndAlso(expression1.Body, invokedExpr), expression1.Parameters);
        }

        public static Expression<Func<TType, bool>> Not<TType>(this Expression<Func<TType, bool>> expression)
        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));

            var negated = Expression.Not(expression.Body);

            return Expression.Lambda<Func<TType, bool>>(negated, expression.Parameters);
        }

        private static Expression<TType> Compose<TType>(this Expression<TType> expression1, Expression<TType> expression2, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of expression2 to parameters of expression1)
            var map = expression1.Parameters
              .Select((first, index)
                => new
                {
                    First = first,
                    Second = expression2.Parameters[index]
                }
              ).ToDictionary(parameter => parameter.Second, parameter => parameter.First);

            // replace parameters in the lambda expression2 with parameters from expression1
            var secondBody = ParameterRebinder.ReplaceParameters(map, expression2.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<TType>(merge.Invoke(expression1.Body, secondBody), expression1.Parameters);
        }
    }
}