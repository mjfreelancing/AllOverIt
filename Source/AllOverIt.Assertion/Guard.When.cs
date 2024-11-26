using System.Diagnostics.CodeAnalysis;
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
        public static TType WhenNotNull<TType>(Expression<Func<TType>> expression, string? errorMessage = default)
            where TType : class
        {
            if (expression is null)
            {
                ThrowArgumentNullException(nameof(expression), errorMessage);
            }

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
        public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(Expression<Func<IEnumerable<TType>>> expression, string? errorMessage = default)
        {
            if (expression is null)
            {
                ThrowArgumentNullException(nameof(expression), errorMessage);
            }

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
        public static IEnumerable<TType>? WhenNotEmpty<TType>(Expression<Func<IEnumerable<TType>>> expression, string? errorMessage = default)
        {
            if (expression is null)
            {
                ThrowArgumentNullException(nameof(expression), errorMessage);
            }

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
        public static string WhenNotNullOrEmpty(Expression<Func<string>> expression, string? errorMessage = default)
        {
            if (expression is null)
            {
                ThrowArgumentNullException(nameof(expression), errorMessage);
            }

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
        public static string? WhenNotEmpty(Expression<Func<string>> expression, string? errorMessage = default)
        {
            if (expression is null)
            {
                ThrowArgumentNullException(nameof(expression), errorMessage);
            }

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
        public static TType WhenNotNull<TType>([NotNull] this TType? @object,
            [CallerArgumentExpression(nameof(@object))] string name = "",
            string? errorMessage = default)
            where TType : class
        {
            if (@object is null)
            {
                ThrowArgumentNullException(name, errorMessage);
            }

            return @object;
        }

        /// <summary>Checks that the provided collection is not null and not empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The collection instance.</param>
        /// <param name="name">The name identifying the collection instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The original object instance when not null and not empty.</returns>
        [return: NotNullIfNotNull(nameof(items))]
        public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(this IEnumerable<TType>? items,
            [CallerArgumentExpression(nameof(items))] string name = "",
            string? errorMessage = default)
        {
            if (items is null)
            {
                ThrowArgumentNullException(name, errorMessage);
            }

            return WhenNotEmpty(items, name, errorMessage);
        }

        /// <summary>Checks that the provided collection is not empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="items">The collection instance.</param>
        /// <param name="name">The name identifying the collection instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <returns>The original collection instance when not empty. If the instance was null then null will be returned.</returns>
        [return: NotNullIfNotNull(nameof(items))]
        public static IEnumerable<TType>? WhenNotEmpty<TType>(this IEnumerable<TType>? items,
            [CallerArgumentExpression(nameof(items))] string name = "",
            string? errorMessage = default)
        {
            if (items is not null)
            {
                var any = items.Any();

                if (!any)
                {
                    ThrowEmptyArgumentException(name, errorMessage);
                }
            }

            return items;
        }

        /// <summary>Checks that the provided string is not null and not empty.</summary>
        /// <param name="value">The string instance.</param>
        /// <param name="name">The name identifying the string instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <returns>The original string instance when not null and not empty.</returns>
        [return: NotNullIfNotNull(nameof(value))]
        public static string? WhenNotNullOrEmpty([NotNull] this string? value,
            [CallerArgumentExpression(nameof(value))] string name = "",
            string? errorMessage = default)
        {
            if (value is null)
            {
                ThrowArgumentNullException(name, errorMessage);
            }

            return WhenNotEmpty(value, name, errorMessage);
        }

        /// <summary>Checks that the provided string is not empty.</summary>
        /// <param name="value">The string instance.</param>
        /// <param name="name">The name identifying the string instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <returns>The original string instance when not empty. If the instance was <see langword="null"/> then <see langword="null"/> will be returned.</returns>
        [return: NotNullIfNotNull(nameof(value))]
        public static string? WhenNotEmpty(this string? value,
            [CallerArgumentExpression(nameof(value))] string name = "",
            string? errorMessage = default)
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
            {
                ThrowEmptyArgumentException(name, errorMessage);
            }

            return value;
        }

        #endregion
    }
}