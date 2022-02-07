using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AllOverIt.Assertion
{
    public static partial class Guard
    {
        #region expression extensions

        /// <summary>Checks that the evaluated expression is not null.</summary>
        /// <typeparam name="TType">The expression's return type.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="errorMessage">The error message to throw if the instance is null. If not provided, the default message
        /// is "Value cannot be null".</param>
        /// <returns>The value of the evaluated expression.</returns>
        /// <remarks>Evaluating the expression is an expensive operation as it must be compiled before it can be invoked.</remarks>
        public static TType WhenNotNull<TType>(Expression<Func<TType>> expression, string errorMessage = default)
            where TType : class
        {
            _ = expression ?? ThrowArgumentNullException<Expression<Func<TType>>>(nameof(expression), errorMessage);

            switch (expression)
            {
                // case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
                case LambdaExpression { Body: MemberExpression memberExpression }:
                    {
                        var value = expression.Compile().Invoke();

                        return value.WhenNotNull(memberExpression.Member.Name, errorMessage);
                    }

                default:
                    throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
            }
        }

        /// <summary>Checks that the evaluated expression is not null and not empty.</summary>
        /// <typeparam name="TType">The expression's return type.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The value of the evaluated expression.</returns>
        /// <remarks>Evaluating the expression is an expensive operation as it must be compiled before it can be invoked.</remarks>
        public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(Expression<Func<IEnumerable<TType>>> expression, string errorMessage = default)
        {
            _ = expression ?? ThrowArgumentNullException<Expression<Func<IEnumerable<TType>>>>(nameof(expression), errorMessage);

            switch (expression)
            {
                // case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
                case LambdaExpression { Body: MemberExpression memberExpression }:
                    {
                        var value = expression.Compile().Invoke();

                        return value.WhenNotNullOrEmpty(memberExpression.Member.Name, errorMessage);
                    }

                default:
                    throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
            }
        }

        /// <summary>Checks that the evaluated expression is not empty.</summary>
        /// <typeparam name="TType">The expression's return type.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="errorMessage">The error message to throw if the instance is null. If not provided, the default message
        /// is "Value cannot be empty".</param>
        /// <returns>The value of the evaluated expression. The evaluated value can be null.</returns>
        /// <remarks>Evaluating the expression is an expensive operation as it must be compiled before it can be invoked.</remarks>
        public static IEnumerable<TType> WhenNotEmpty<TType>(Expression<Func<IEnumerable<TType>>> expression, string errorMessage = default)
        {
            _ = expression ?? ThrowArgumentNullException<Expression<Func<IEnumerable<TType>>>>(nameof(expression), errorMessage);

            switch (expression)
            {
                // case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
                case LambdaExpression { Body: MemberExpression memberExpression }:
                    {
                        var value = expression.Compile().Invoke();

                        return value.WhenNotEmpty(memberExpression.Member.Name, errorMessage);
                    }

                default:
                    throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
            }
        }

        /// <summary>Checks that the evaluated expression is not null and not empty.</summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The value of the evaluated expression.</returns>
        /// <remarks>Evaluating the expression is an expensive operation as it must be compiled before it can be invoked.</remarks>
        public static string WhenNotNullOrEmpty(Expression<Func<string>> expression, string errorMessage = default)
        {
            _ = expression ?? ThrowArgumentNullException<Expression<Func<string>>>(nameof(expression), errorMessage);

            switch (expression)
            {
                // case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
                case LambdaExpression { Body: MemberExpression memberExpression }:
                    {
                        var value = expression.Compile().Invoke();

                        return value.WhenNotNullOrEmpty(memberExpression.Member.Name, errorMessage);
                    }

                default:
                    throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
            }
        }

        /// <summary>Checks that the evaluated expression is not empty.</summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="errorMessage">The error message to throw if the instance is null. If not provided, the default message
        /// is "Value cannot be empty".</param>
        /// <returns>The value of the evaluated expression. The evaluated value can be null.</returns>
        /// <remarks>Evaluating the expression is an expensive operation as it must be compiled before it can be invoked.</remarks>
        public static string WhenNotEmpty(Expression<Func<string>> expression, string errorMessage = default)
        {
            _ = expression ?? ThrowArgumentNullException<Expression<Func<string>>>(nameof(expression), errorMessage);

            switch (expression)
            {
                // case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
                case LambdaExpression { Body: MemberExpression memberExpression }:
                    {
                        var value = expression.Compile().Invoke();

                        return value.WhenNotEmpty(memberExpression.Member.Name, errorMessage);
                    }

                default:
                    throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
            }
        }

        #endregion

        #region object extensions

        /// <summary>Checks that the provided object is not null.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="name">The name identifying the object instance.</param>
        /// <param name="errorMessage">The error message to throw if the instance is null. If not provided, the default message
        /// is "Value cannot be null".</param>
        /// <returns>The original object instance when not null.</returns>
        public static TType WhenNotNull<TType>(this TType @object,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("object")]
#endif
            string name = "",
            string errorMessage = default)
            where TType : class
        {
            return @object ?? ThrowArgumentNullException<TType>(name, errorMessage);
        }

        /// <summary>Checks that the provided collection is not null and not empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The collection instance.</param>
        /// <param name="name">The name identifying the collection instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The original object instance when not null and not empty.</returns>
        /// <remarks>This method also validates that the enumerable is an array or ICollection&lt;TType&gt; to prevent multiple enumeration of the IEnumerable.</remarks>
        public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(this IEnumerable<TType> @object,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("object")]
#endif
            string name = "",
            string errorMessage = default)
        {
            return WhenNotNullOrEmpty(@object, true, name, errorMessage);
        }

        /// <summary>Checks that the provided collection is not null and not empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The collection instance.</param>
        /// <param name="ensureIsCollection">Indicates if the method should also validate that the enumerable is an array or ICollection&lt;TType&gt;
        /// to prevent multiple enumeration of the IEnumerable.</param>
        /// <param name="name">The name identifying the collection instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The original object instance when not null and not empty.</returns>
        public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(this IEnumerable<TType> @object, bool ensureIsCollection,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("object")]
#endif
            string name = "",
            string errorMessage = default)
        {
            _ = @object ?? ThrowArgumentNullException<IEnumerable<TType>>(name, errorMessage);

            return WhenNotEmpty(@object, ensureIsCollection, name, errorMessage);
        }

        /// <summary>Checks that the provided collection is not empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The collection instance.</param>
        /// <param name="name">The name identifying the collection instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The original collection instance when not empty. If the instance was null then null will be returned.</returns>
        /// <remarks>This method also validates that the enumerable is an array or ICollection&lt;TType&gt; to prevent multiple enumeration of the IEnumerable.</remarks>
        public static IEnumerable<TType> WhenNotEmpty<TType>(this IEnumerable<TType> @object,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("object")]
#endif
            string name = "",
            string errorMessage = default)
        {
            return WhenNotEmpty<TType>(@object, true, name, errorMessage);
        }

        /// <summary>Checks that the provided collection is not empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The collection instance.</param>
        /// <param name="ensureIsCollection">Indicates if the method should also validate that the enumerable is an array or ICollection&lt;TType&gt;
        /// to prevent multiple enumeration of the IEnumerable.</param>
        /// <param name="name">The name identifying the collection instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <returns>The original collection instance when not empty. If the instance was null then null will be returned.</returns>
        public static IEnumerable<TType> WhenNotEmpty<TType>(this IEnumerable<TType> @object, bool ensureIsCollection,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("object")]
#endif
            string name = "",
            string errorMessage = default)
        {
            if (@object != null)
            {
                if (ensureIsCollection)
                {
                    var isCollection = @object is ICollection<TType>;
                    var isArray = @object.GetType().IsArray;

                    if (!(isCollection || isArray))
                    {
                        throw new InvalidOperationException($"Expecting an array or ICollection<T> (Parameter '{name}')");
                    }
                }

                // ReSharper disable once PossibleMultipleEnumeration
                if (!@object.Any())
                    // ReSharper disable once PossibleMultipleEnumeration
                {
                    throw new ArgumentException(errorMessage ?? "The argument cannot be empty", name);
                }
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return @object;
        }

        /// <summary>Checks that the provided string is not null and not empty.</summary>
        /// <param name="object">The string instance.</param>
        /// <param name="name">The name identifying the string instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The original string instance when not null and not empty.</returns>
        public static string WhenNotNullOrEmpty(this string @object,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("object")]
#endif
            string name = "",
            string errorMessage = default)
        {
            _ = @object ?? ThrowArgumentNullException(name, errorMessage);

            return WhenNotEmpty(@object, name, errorMessage);
        }

        /// <summary>Checks that the provided string is not empty.</summary>
        /// <param name="object">The string instance.</param>
        /// <param name="name">The name identifying the string instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <returns>The original string instance when not empty.</returns>
        public static string WhenNotEmpty(this string @object,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("object")]
#endif
            string name = "",
            string errorMessage = default)
        {
            if (@object == null || !string.IsNullOrWhiteSpace(@object))
            {
                return @object;
            }

            throw new ArgumentException(errorMessage ?? "The argument cannot be empty", name);
        }

        #endregion
    }
}