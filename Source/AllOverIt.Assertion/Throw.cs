﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Assertion
{
    /// <summary>Static helper class that will throw a specified exception type when a given criteria is satisfied.</summary>
    /// <typeparam name="TException">The exception type to be thrown.</typeparam>
    public static class Throw<TException> where TException : Exception
    {
        #region When
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is true.</summary>
        /// <param name="condition">The predicate condition.</param>
        public static void When(bool condition)
        {
            if (condition)
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is true.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void When<TExceptionArg1>(bool condition, TExceptionArg1 arg1)
        {
            if (condition)
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is true.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void When<TExceptionArg1, TExceptionArg2>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2)
        {
            if (condition)
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is true.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void When<TExceptionArg1, TExceptionArg2, TExceptionArg3>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)
        {
            if (condition)
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is true.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        /// <param name="arg4">The fourth argument to be provided to the exception constructor.</param>
        public static void When<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
        {
            if (condition)
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNot
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is false.</summary>
        /// <param name="condition">The predicate condition.</param>
        public static void WhenNot(bool condition)
        {
            if (!condition)
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is false.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1>(bool condition, TExceptionArg1 arg1)
        {
            if (!condition)
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is false.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is false.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is false.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        /// <param name="arg4">The fourth argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNull
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        public static void WhenNull<TType>(TType @object)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1>(TType @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        /// <param name="arg4">The fourth argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNotNull
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        public static void WhenNotNull<TType>(TType @object)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNotNull<TType, TExceptionArg1>(TType @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNotNull<TType, TExceptionArg1, TExceptionArg2>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNotNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        /// <param name="arg4">The fourth argument to be provided to the exception constructor.</param>
        public static void WhenNotNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNullOrEmpty
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <param name="object">The object instance.</param>
        public static void WhenNullOrEmpty(string @object)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TExceptionArg1>(string @object, TExceptionArg1 arg1)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TExceptionArg1, TExceptionArg2>(string @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TExceptionArg1, TExceptionArg2, TExceptionArg3>(string @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        /// <param name="arg4">The fourth argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(string @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNullOrEmpty
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        public static void WhenNullOrEmpty<TType>(IEnumerable<TType> @object)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TType, TExceptionArg1>(IEnumerable<TType> @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TType, TExceptionArg1, TExceptionArg2>(IEnumerable<TType> @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(IEnumerable<TType> @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is null or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        /// <param name="arg4">The fourth argument to be provided to the exception constructor.</param>
        public static void WhenNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(IEnumerable<TType> @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region ThrowException
        private static void ThrowException()
        {
            throw (Exception) Activator.CreateInstance(typeof(TException));
        }

        private static void ThrowException<TArg1>(TArg1 arg1)
        {
            throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1 });
        }

        private static void ThrowException<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1, arg2 });
        }

        private static void ThrowException<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1, arg2, arg3 });
        }

        private static void ThrowException<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            throw (Exception) Activator.CreateInstance(typeof(TException), new object[] { arg1, arg2, arg3, arg4 });
        }
        #endregion

        private static bool IsNull<TType>(TType @object)
        {
            return @object is null;
        }

        private static bool IsNotNull<TType>(TType @object)
        {
            // This is more efficient that !IsNull()
            return @object is not null;
        }

        private static bool IsNullOrEmpty(string @object)
        {
            return string.IsNullOrWhiteSpace(@object);
        }

        private static bool IsNullOrEmpty<TType>(IEnumerable<TType> @object)
        {
            return @object is null || !@object.Any();
        }
    }
}