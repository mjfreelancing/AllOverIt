using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Assertion
{
    public static partial class Guard
    {
        /// <summary>Represents a type that will throw an exception.</summary>
        public interface IThrowWhen
        {
            void Throw<TException>(string errorMessage = default) where TException : Exception;
        }

        /// <summary>Provides the facility to throw an exception if a condition is true.</summary>
        public sealed class ThrowWhen
        {
            private sealed class DoThrow : IThrowWhen
            {
                public void Throw<TException>(string errorMessage = default) where TException : Exception
                {
                    throw CreateException<TException>(errorMessage);
                }

                private static Exception CreateException<TException>(string errorMessage) where TException : Exception
                {
                    return string.IsNullOrWhiteSpace(errorMessage)
                        ? (Exception) Activator.CreateInstance(typeof(TException))
                        : (Exception) Activator.CreateInstance(typeof(TException), new object[] { errorMessage });
                }
            }

            private sealed class DoNotThrow : IThrowWhen
            {
                public void Throw<TException>(string errorMessage = default) where TException : Exception
                {
                    // Do nothing
                }
            }

            private static readonly IThrowWhen DoThrowInstance = new DoThrow();
            private static readonly IThrowWhen DoNotThrowInstance = new DoNotThrow();

            /// <summary>Factory method to create a <see cref="ThrowWhen"/> instance.</summary>
            /// <param name="condition">Indicates if an exception should be thrown when the <see cref="ThrowWhen.Throw{TException}(string)"/> method is called.</param>
            /// <returns>A <see cref="ThrowWhen"/> instance.</returns>
            public static IThrowWhen Create(bool condition)
            {
                return condition
                    ? DoThrowInstance
                    : DoNotThrowInstance;
            }
        }

        /// <summary>Sets up a precondition to throw an exception when a class instance is null.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The class instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen WhenNull<TType>(this TType @object)
            where TType : class
        {
            return ThrowWhen.Create(@object is null);
        }

        /// <summary>Sets up a precondition to throw an exception when .</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The class instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen WhenNotNull<TType>(this TType @object)
            where TType : class
        {
            return ThrowWhen.Create(@object is not null);
        }

        /// <summary>Sets up a precondition to throw an exception when a string is null.</summary>
        /// <param name="object">The string instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen WhenNull(this string @object)
        {
            return ThrowWhen.Create(@object is null);
        }

        /// <summary>Sets up a precondition to throw an exception when a string is null or empty (whitespace).</summary>
        /// <param name="object">The string instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen WhenNullOrEmpty(this string @object)
        {
            return ThrowWhen.Create(string.IsNullOrWhiteSpace(@object));
        }

        /// <summary>Sets up a precondition to throw an exception when a string is not null or empty (whitespace).</summary>
        /// <param name="object">The string instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen WhenNotNullOrEmpty(this string @object)
        {
            return ThrowWhen.Create(!string.IsNullOrWhiteSpace(@object));
        }

        /// <summary>Sets up a precondition to throw an exception when .</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The class instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen WhenNullOrEmpty<TType>(this IEnumerable<TType> @object)
        {
            return ThrowWhen.Create(@object is null || !@object.Any());
        }

        /// <summary>Sets up a precondition to throw an exception when .</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The class instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen WhenNotNullOrEmpty<TType>(this IEnumerable<TType> @object)
        {
            return ThrowWhen.Create(@object is not null && @object.Any());
        }
    }
}