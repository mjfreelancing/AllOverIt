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
            void Exception(Exception exception);
            void Exception<TException>() where TException : Exception, new();
            void Exception<TException>(string message) where TException : Exception;
            void Exception<TException, TArg1>(TArg1 arg1) where TException : Exception;
            void Exception<TException, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where TException : Exception;
            void Exception<TException, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where TException : Exception;
            void Exception<TException, TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TException : Exception;
        }

        /// <summary>Provides the facility to throw an exception if a condition is true.</summary>
        public sealed class ThrowWhen
        {
            private sealed class DoThrow : IThrowWhen
            {
                public void Exception(Exception exception)
                {
                    throw exception;
                }

                public void Exception<TException>() where TException : Exception, new()
                {
                    throw new TException();
                }

                public void Exception<TException>(string message) where TException : Exception
                {
                    Exception<TException, string>(message);
                }

                public void Exception<TException, TArg1>(TArg1 arg1) where TException : Exception
                {
                    throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1 });
                }

                public void Exception<TException, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where TException : Exception
                {
                    throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1, arg2 });
                }

                public void Exception<TException, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where TException : Exception
                {
                    throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1, arg2, arg3 });
                }

                public void Exception<TException, TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TException : Exception
                {
                    throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1, arg2, arg3, arg4 });
                }
            }

            private sealed class DoNotThrow : IThrowWhen
            {
                public void Exception(Exception exception) { }
                public void Exception<TException>() where TException : Exception, new() { }
                public void Exception<TException>(string message) where TException : Exception { }
                public void Exception<TException, TArg1>(TArg1 arg1) where TException : Exception { }
                public void Exception<TException, TArg1, TArg2>(TArg1 arg1, TArg2 arg2) where TException : Exception { }
                public void Exception<TException, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3) where TException : Exception { }
                public void Exception<TException, TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where TException : Exception { }
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
        public static IThrowWhen ThrowWhenNull<TType>(this TType @object)
            where TType : class
        {
            return ThrowWhen.Create(@object is null);
        }

        /// <summary>Sets up a precondition to throw an exception when .</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The class instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen ThrowWhenNotNull<TType>(this TType @object)
            where TType : class
        {
            return ThrowWhen.Create(@object is not null);
        }

        /// <summary>Sets up a precondition to throw an exception when a string is null.</summary>
        /// <param name="object">The string instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen ThrowWhenNull(this string @object)
        {
            return ThrowWhen.Create(@object is null);
        }

        /// <summary>Sets up a precondition to throw an exception when a string is null or empty (whitespace).</summary>
        /// <param name="object">The string instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen ThrowWhenNullOrEmpty(this string @object)
        {
            return ThrowWhen.Create(string.IsNullOrWhiteSpace(@object));
        }

        /// <summary>Sets up a precondition to throw an exception when a string is not null or empty (whitespace).</summary>
        /// <param name="object">The string instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen ThrowWhenNotNullOrEmpty(this string @object)
        {
            return ThrowWhen.Create(!string.IsNullOrWhiteSpace(@object));
        }

        /// <summary>Sets up a precondition to throw an exception when .</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The class instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen ThrowWhenNullOrEmpty<TType>(this IEnumerable<TType> @object)
        {
            return ThrowWhen.Create(@object is null || !@object.Any());
        }

        /// <summary>Sets up a precondition to throw an exception when .</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The class instance.</param>
        /// <returns>A <see cref="ThrowWhen"/> object that provides the ability to throw an exception when a condition is satisfied.</returns>
        public static IThrowWhen ThrowWhenNotNullOrEmpty<TType>(this IEnumerable<TType> @object)
        {
            return ThrowWhen.Create(@object is not null && @object.Any());
        }
    }
}