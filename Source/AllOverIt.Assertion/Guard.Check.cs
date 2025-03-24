using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AllOverIt.Assertion
{
    public static partial class Guard
    {
        /// <summary>Checks the specified object is not null, throwing <see cref="InvalidOperationException"/> if it is.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="name">The name of the instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null".</param>
        /// <exception cref="InvalidOperationException"/>
        public static void CheckNotNull<TType>([NotNull] this TType? @object,
            [CallerArgumentExpression(nameof(@object))] string name = "",
            string? errorMessage = default)
            where TType : class
        {
            if (@object is null)
            {
                ThrowInvalidOperationException(name, errorMessage ?? "Value cannot be null");
            }
        }

        /// <summary>Checks the specified object is null, throwing <see cref="InvalidOperationException"/> if it isn't.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="name">The name of the instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value must be null".</param>
        /// <exception cref="InvalidOperationException"/>
        public static void CheckIsNull<TType>(this TType? @object,
            [CallerArgumentExpression(nameof(@object))] string name = "",
            string? errorMessage = default)
            where TType : class
        {
            if (@object is not null)
            {
                ThrowInvalidOperationException(name, errorMessage ?? "Value must be null");
            }
        }

        /// <summary>Checks the specified enumerable is not null or empty, throwing <see cref="InvalidOperationException"/> if it is.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The enumerable instance</param>
        /// <param name="name">The name of the instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <remarks>If <paramref name="object"/> is a lazily-evaluated enumerable, such as the result if a LINQ Select() then multiple
        /// enumeration will occur. Only pass a concrete enumerable, such as an array or list, to this method.</remarks>
        /// <exception cref="InvalidOperationException"/>
        public static void CheckNotNullOrEmpty<TType>([NotNull] this IEnumerable<TType>? @object,
            [CallerArgumentExpression(nameof(@object))] string name = "",
            string? errorMessage = default)
        {
            CheckNotNull(@object, name, errorMessage);
            CheckNotEmpty(@object!, name, errorMessage);
        }

        /// <summary>Checks the specified enumerable is not empty, throwing <see cref="InvalidOperationException"/> if it isn't.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The enumerable instance.</param>
        /// <param name="name">The name of the instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <remarks>If <paramref name="object"/> is a lazily-evaluated enumerable, such as the result if a LINQ Select() then multiple
        /// enumeration will occur. Only pass a concrete enumerable, such as an array or list, to this method.</remarks>
        /// <exception cref="InvalidOperationException"/>
        public static void CheckNotEmpty<TType>(this IEnumerable<TType> @object,
            [CallerArgumentExpression(nameof(@object))] string name = "",
            string? errorMessage = default)
        {
            if (@object is not null && !@object.Any())
            {
                ThrowInvalidOperationException(name, errorMessage ?? "Value cannot be empty");
            }
        }

        /// <summary>Checks the specified string is not null or empty, throwing <see cref="InvalidOperationException"/> if it is.</summary>
        /// <param name="object">The string instance</param>
        /// <param name="name">The name of the instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty string.</param>
        /// <exception cref="InvalidOperationException"/>
        public static void CheckNotNullOrEmpty([NotNull] this string? @object,
            [CallerArgumentExpression(nameof(@object))] string name = "",
            string? errorMessage = default)
        {
            CheckNotNull(@object, name, errorMessage);
            CheckNotEmpty(@object!, name, errorMessage);
        }

        /// <summary>Checks the specified string is not empty, throwing <see cref="InvalidOperationException"/> if it isn't.</summary>
        /// <param name="object">The string instance</param>
        /// <param name="name">The name of the instance.</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <exception cref="InvalidOperationException"/>
        public static void CheckNotEmpty(this string @object,
            [CallerArgumentExpression(nameof(@object))] string name = "",
            string? errorMessage = default)
        {
            if (@object is not null && string.IsNullOrWhiteSpace(@object))
            {
                ThrowInvalidOperationException(name, errorMessage ?? "Value cannot be empty");
            }
        }
    }
}