using AllOverIt.Expressions;
using System.Linq.Expressions;

namespace AllOverIt.Helpers
{
    /// <summary>Provides a compiled type factory for instantiating types based on constructor argument types.</summary>
    /// <typeparam name="TCreate">The type to create.</typeparam>
    public static class TypeFactory<TCreate>
    {
        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> using its default constructor.</summary>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance.</returns>
        public static Func<TCreate> Create()
        {
            var newExpression = Expression.New(typeof(TCreate));

            return Expression
                .Lambda<Func<TCreate>>(newExpression)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TCreate> Create<TArg1>()
        {
            var paramTypes = new[] { typeof(TArg1) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, TCreate> Create(Type arg1)
        {
            var paramTypes = new[] { arg1 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TArg2, TCreate> Create<TArg1, TArg2>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, object, TCreate> Create(Type arg1, Type arg2)
        {
            var paramTypes = new[] { arg1, arg2 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, object, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TArg2, TArg3, TCreate> Create<TArg1, TArg2, TArg3>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, object, object, TCreate> Create(Type arg1, Type arg2, Type arg3)
        {
            var paramTypes = new[] { arg1, arg2, arg3 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, object, object, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TCreate> Create<TArg1, TArg2, TArg3, TArg4>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, object, object, object, TCreate> Create(Type arg1, Type arg2, Type arg3, Type arg4)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, object, object, object, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, object, object, object, object, TCreate> Create(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, object, object, object, object, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <typeparam name="TArg6">Constructor argument 6.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <param name="arg6">Constructor argument 6.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, object, object, object, object, object, TCreate> Create(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5, arg6 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, object, object, object, object, object, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <typeparam name="TArg6">Constructor argument 6.</typeparam>
        /// <typeparam name="TArg7">Constructor argument 7.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <param name="arg6">Constructor argument 6.</param>
        /// <param name="arg7">Constructor argument 7.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, object, object, object, object, object, object, TCreate> Create(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, object, object, object, object, object, object, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor argument types.</summary>
        /// <typeparam name="TArg1">Constructor argument 1.</typeparam>
        /// <typeparam name="TArg2">Constructor argument 2.</typeparam>
        /// <typeparam name="TArg3">Constructor argument 3.</typeparam>
        /// <typeparam name="TArg4">Constructor argument 4.</typeparam>
        /// <typeparam name="TArg5">Constructor argument 5.</typeparam>
        /// <typeparam name="TArg6">Constructor argument 6.</typeparam>
        /// <typeparam name="TArg7">Constructor argument 7.</typeparam>
        /// <typeparam name="TArg8">Constructor argument 8.</typeparam>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TCreate> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>()
        {
            var paramTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7), typeof(TArg8) };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParameters(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TCreate>>(newExpression, parameters)
                .Compile();
        }

        /// <summary>Creates a compiled factory that when invoked will instantiate a <typeparamref name="TCreate"/> with the specified constructor
        /// argument types. All arguments can be provided to the factory as object types but they will be converted to the required constructor
        /// argument type.</summary>
        /// <param name="arg1">Constructor argument 1.</param>
        /// <param name="arg2">Constructor argument 2.</param>
        /// <param name="arg3">Constructor argument 3.</param>
        /// <param name="arg4">Constructor argument 4.</param>
        /// <param name="arg5">Constructor argument 5.</param>
        /// <param name="arg6">Constructor argument 6.</param>
        /// <param name="arg7">Constructor argument 7.</param>
        /// <param name="arg8">Constructor argument 8.</param>
        /// <returns>A factory that when invoked will return a newly constructed <typeparamref name="TCreate"/> instance using provided arguments.</returns>
        public static Func<object, object, object, object, object, object, object, object, TCreate> Create(Type arg1, Type arg2, Type arg3, Type arg4, Type arg5, Type arg6, Type arg7, Type arg8)
        {
            var paramTypes = new[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };

            var (newExpression, parameters) = ExpressionUtils.GetConstructorWithParametersAsObjects(typeof(TCreate), paramTypes);

            return Expression
                .Lambda<Func<object, object, object, object, object, object, object, object, TCreate>>(newExpression, parameters)
                .Compile();
        }
    }
}