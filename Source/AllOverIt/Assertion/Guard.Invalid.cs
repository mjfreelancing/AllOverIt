using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Assertion
{
    public static partial class Guard
    {
        /// <summary>Throws an exception if the object instance is null, otherwise it returns the same instance.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="errorMessage">The error message to throw if the instance is null. If not provided, the default message
        /// is "Value cannot be null".</param>
        /// <returns>The same source object instance.</returns>
        public static TType InvalidWhenNull<TType>(this TType @object, string errorMessage = default)
            where TType : class
        {
            return @object ?? ThrowInvalidOperationException<TType>(errorMessage ?? "Value cannot be null");
        }

        /// <summary>Throws an exception if the object instance is not null, otherwise it returns the same instance.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="errorMessage">The error message to throw if the instance is not null. If not provided, the default message
        /// is "Value must be null".</param>
        /// <returns>The same source object instance.</returns>
        public static TType InvalidWhenNotNull<TType>(this TType @object, string errorMessage = default)
            where TType : class
        {
            if (@object != null)
            {
                ThrowInvalidOperationException<TType>(errorMessage ?? "Value must be null");
            }

            return @object;
        }

        /// <summary>Throws an exception if the specified enumerable is null or empty, otherwise it returns the same instance.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The enumerable instance</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty collection.</param>
        /// <remarks>If <paramref name="object"/> is a lazily-evaluated enumerable, such as the result if a LINQ Select() then multiple
        /// enumeration will occur. Only pass a concrete enumerable, such as an array or list, to this method.</remarks>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>The same source object instance.</returns>
        public static IEnumerable<TType> InvalidWhenNullOrEmpty<TType>(this IEnumerable<TType> @object, string errorMessage = default)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            InvalidWhenNull(@object, errorMessage);

            // ReSharper disable once PossibleMultipleEnumeration
            return InvalidWhenEmpty(@object, errorMessage);
        }

        /// <summary>Throws an exception if the specified enumerable is empty, otherwise it returns the same instance.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The enumerable instance</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <remarks>If <paramref name="object"/> is a lazily-evaluated enumerable, such as the result if a LINQ Select() then multiple
        /// enumeration will occur. Only pass a concrete enumerable, such as an array or list, to this method.</remarks>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>The same source object instance.</returns>
        public static IEnumerable<TType> InvalidWhenEmpty<TType>(this IEnumerable<TType> @object, string errorMessage = default)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (@object != null && !@object.Any())
            {
                ThrowInvalidOperationException<IEnumerable<TType>>(errorMessage ?? "Value cannot be empty");
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return @object;
        }

        /// <summary>Throws an exception if the specified string is null or empty, otherwise it returns the same instance.</summary>
        /// <param name="object">The string instance</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be null" for a null
        /// instance and "Value cannot be empty" for an empty string.</param>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>The same source string instance.</returns>
        public static string InvalidWhenNullOrEmpty(this string @object, string errorMessage = default)
        {
            CheckNotNull(@object, errorMessage);

            return InvalidWhenEmpty(@object, errorMessage);
        }

        /// <summary>Throws an exception if the specified string is empty, otherwise it returns the same instance.</summary>
        /// <param name="object">The string instance</param>
        /// <param name="errorMessage">The error message to report. If not provided, the default message is "Value cannot be empty".</param>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>The same source string instance.</returns>
        public static string InvalidWhenEmpty(this string @object, string errorMessage = default)
        {
            if (@object != null && string.IsNullOrWhiteSpace(@object))
            {
                ThrowInvalidOperationException<string>(errorMessage ?? "Value cannot be empty");
            }

            return @object;
        }
    }
}