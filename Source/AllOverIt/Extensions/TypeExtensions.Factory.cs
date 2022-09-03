using AllOverIt.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Extensions
{
    // TODO: tests

    public static partial class TypeExtensions
    {
        public static Func<object> GetFactory(this Type type)
        {
            var newExpression = Expression.New(type);

            return Expression
                .Lambda<Func<object>>(newExpression)
                .Compile();
        }

        public static Func<TArg1, object> GetFactory<TArg1>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, object>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, object> GetFactory<TArg1, TArg2>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, TArg2, object>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, object> GetFactory<TArg1, TArg2, TArg3>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, object>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, object> GetFactory<TArg1, TArg2, TArg3, TArg4>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, object>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, object>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, object>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, object>>(newExpression, parameters)
                .Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7), typeof(TArg8) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, paramTypes.ToArray());

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, object>>(newExpression, parameters)
                .Compile();
        }
    }
}