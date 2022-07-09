using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Assertion
{
    public static class Throw<TException> where TException : Exception
    {
        #region When
        public static void When(bool condition)
        {
            if (condition)
            {
                ThrowException();
            }
        }

        public static void When<TExceptionArg1>(bool condition, TExceptionArg1 arg1)
        {
            if (condition)
            {
                ThrowException(arg1);
            }
        }

        public static void When<TExceptionArg1, TExceptionArg2>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2)
        {
            if (condition)
            {
                ThrowException(arg1, arg2);
            }
        }

        public static void When<TExceptionArg1, TExceptionArg2, TExceptionArg3>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3)
        {
            if (condition)
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        public static void When<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3, TExceptionArg2 arg4)
        {
            if (condition)
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNot
        public static void WhenNot(bool condition)
        {
            if (!condition)
            {
                ThrowException();
            }
        }

        public static void WhenNot<TExceptionArg1>(bool condition, TExceptionArg1 arg1)
        {
            if (!condition)
            {
                ThrowException(arg1);
            }
        }

        public static void WhenNot<TExceptionArg1, TExceptionArg2>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2);
            }
        }

        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3, TExceptionArg2 arg4)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNull
        public static void WhenNull<TType>(TType @object)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException();
            }
        }

        public static void WhenNull<TType, TExceptionArg1>(TType @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1);
            }
        }

        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3, TExceptionArg2 arg4)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNotNull
        public static void WhenNotNull<TType>(TType @object)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException();
            }
        }

        public static void WhenNotNull<TType, TExceptionArg1>(TType @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1);
            }
        }

        public static void WhenNotNull<TType, TExceptionArg1, TExceptionArg2>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        public static void WhenNotNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        public static void WhenNotNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(TType @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3, TExceptionArg2 arg4)
            where TType : class
        {
            if (IsNotNull(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNullOrEmpty
        public static void WhenNullOrEmpty(string @object)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException();
            }
        }

        public static void WhenNullOrEmpty<TExceptionArg1>(string @object, TExceptionArg1 arg1)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1);
            }
        }

        public static void WhenNullOrEmpty<TExceptionArg1, TExceptionArg2>(string @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        public static void WhenNullOrEmpty<TExceptionArg1, TExceptionArg2, TExceptionArg3>(string @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        public static void WhenNullOrEmpty<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(string @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3, TExceptionArg2 arg4)
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
        #endregion

        #region WhenNullOrEmpty
        public static void WhenNullOrEmpty<TType>(IEnumerable<TType> @object)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException();
            }
        }

        public static void WhenNullOrEmpty<TType, TExceptionArg1>(IEnumerable<TType> @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1);
            }
        }

        public static void WhenNullOrEmpty<TType, TExceptionArg1, TExceptionArg2>(IEnumerable<TType> @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        public static void WhenNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(IEnumerable<TType> @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3)
            where TType : class
        {
            if (IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        public static void WhenNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(IEnumerable<TType> @object, TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg2 arg3, TExceptionArg2 arg4)
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