using AllOverIt.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Extensions
{
    public static partial class TypeExtensions
    {
        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> using its default constructor.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object> GetFactory(this Type type)
        {
            var newExpression = Expression.New(type);

            return Expression
                .Lambda<Func<object>>(newExpression)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, object> GetFactory<TArg1>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object> GetFactory(this Type type, Type arg1)
        {
            var paramTypes = new[] { arg1 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, TArg2, object> GetFactory<TArg1, TArg2>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, TArg2, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object, object> GetFactory(this Type type, Type arg1, Type arg2)
        {
            var paramTypes = new[] { arg1, arg2 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, TArg2, TArg3, object> GetFactory<TArg1, TArg2, TArg3>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object, object, object> GetFactory(this Type type, Type arg1, Type arg2, Type arg3)
        {
            var paramTypes = new[] { arg1, arg2, arg3 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object, object, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, object> GetFactory<TArg1, TArg2, TArg3, TArg4>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object, object, object, object> GetFactory(this Type type, Type arg1, Type arg2, Type arg3, Type arg4)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object, object, object, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object, object, object, object, object> GetFactory(this Type type, Type arg1, Type arg2, Type arg3, Type arg4, Type arg5)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object, object, object, object, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <typeparam name="TArg6">Constructor argument 6.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <param name="arg6">Constructor argument 6.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object, object, object, object, object, object> GetFactory(this Type type, Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5, arg6 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object, object, object, object, object, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <typeparam name="TArg6">Constructor argument 6.</typeparam>
        /// <typeparam name="TArg7">Constructor argument 7.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <param name="arg6">Constructor argument 6.</param>
        /// <param name="arg7">Constructor argument 7.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object, object, object, object, object, object, object> GetFactory(this Type type, Type arg1, Type arg2, Type arg3, Type arg4, Type arg5,
            Type arg6, Type arg7)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object, object, object, object, object, object, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <typeparam name="TArg6">Constructor argument 6.</typeparam>
        /// <typeparam name="TArg7">Constructor argument 7.</typeparam>
        /// <typeparam name="TArg8">Constructor argument 8.</typeparam>
        /// <param name="type">The object type to create a factory for.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, object> GetFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(this Type type)
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7), typeof(TArg8) };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, object>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate the <paramref name="type"/> with the specified constructor argument types.</summary>
        /// <param name="type">The object type to create a factory for.</param>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <param name="arg6">Constructor argument 6.</param>
        /// <param name="arg7">Constructor argument 7.</param>
        /// <param name="arg8">Constructor argument 8.</param>
        /// <returns>A factory that when invoked will return a newly constructed <paramref name="type"/> instance.</returns>
        public static Func<object, object, object, object, object, object, object, object, object> GetFactory(this Type type, Type arg1, Type arg2, Type arg3, Type arg4,
            Type arg5, Type arg6, Type arg7, Type arg8)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(type, [.. paramTypes]);

            return Expression
                .Lambda<Func<object, object, object, object, object, object, object, object, object>>(newExpression, parameters)
                .Compile();
        }
    }
}