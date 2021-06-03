using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Helpers
{
    public static partial class Guard
    {
        #region expression extensions

        /// <summary>Checks that the evaluated expression is not null.</summary>
        /// <remarks>The expression argument cannot be null.</remarks>
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
        /// <remarks>The expression argument cannot be null.</remarks>
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
        /// <remarks>The expression argument cannot be null.</remarks>
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
        /// <remarks>The expression argument cannot be null.</remarks>
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
        /// <remarks>The expression argument cannot be null.</remarks>
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

        // returns argument if not null, otherwise throws ArgumentNullException
        public static TType WhenNotNull<TType>(this TType argument, string name, string errorMessage = default)
            where TType : class
        {
            return argument ?? ThrowArgumentNullException<TType>(name, errorMessage);
        }

        // returns argument if not null or empty, otherwise throws ArgumentNullException / ArgumentException
        public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(this IEnumerable<TType> argument, string name, string errorMessage = default)
        {
            _ = argument ?? ThrowArgumentNullException<IEnumerable<TType>>(name, errorMessage);

            return WhenNotEmpty(argument, name, errorMessage);
        }

        // returns argument if null or not empty, otherwise throws ArgumentException
        public static IEnumerable<TType> WhenNotEmpty<TType>(this IEnumerable<TType> argument, string name, string errorMessage = default)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (argument != null && !argument.Any())
            {
                throw new ArgumentException(errorMessage ?? "The argument cannot be empty", name);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return argument;
        }

        // returns argument if not null or empty, otherwise throws ArgumentNullException / ArgumentException
        public static string WhenNotNullOrEmpty(this string argument, string name, string errorMessage = default)
        {
            _ = argument ?? ThrowArgumentNullException(name, errorMessage);

            return WhenNotEmpty(argument, name, errorMessage);
        }

        // returns argument if null or not empty, otherwise throws ArgumentNullException / ArgumentException
        public static string WhenNotEmpty(this string argument, string name, string errorMessage = default)
        {
            if (argument == null || !string.IsNullOrWhiteSpace(argument))
            {
                return argument;
            }

            throw new ArgumentException(errorMessage ?? "The argument cannot be empty", name);
        }

        #endregion
    }
}