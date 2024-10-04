using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Assertion
{
    /// <summary>Static helper class that will throw a specified exception type when a given criteria is satisfied.</summary>
    /// <typeparam name="TException">The exception type to be thrown.</typeparam>
    public static partial class Throw<TException> where TException : Exception
    {
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        public static void WhenNull<TType>([NotNull] TType? @object)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1>([NotNull] TType? @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">A Func that returns the argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1>([NotNull] TType? @object, Func<TExceptionArg1> arg1)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1.Invoke());
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2>([NotNull] TType? @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="args">A Func that returns the two arguments to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2>([NotNull] TType? @object, Func<(TExceptionArg1 arg1, TExceptionArg2 arg2)> args)
            where TType : class
        {
            if (IsNull(@object))
            {
                var (arg1, arg2) = args.Invoke();

                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>([NotNull] TType? @object, TExceptionArg1 arg1,
            TExceptionArg2 arg2, TExceptionArg3 arg3)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="args">A Func that returns the three arguments to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>([NotNull] TType? @object,
            Func<(TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)> args)
            where TType : class
        {
            if (IsNull(@object))
            {
                var (arg1, arg2, arg3) = args.Invoke();

                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
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
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>([NotNull] TType? @object,
            TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
            where TType : class
        {
            if (IsNull(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is <see langword="null"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="args">A Func that returns the four arguments to be provided to the exception constructor.</param>
        public static void WhenNull<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>([NotNull] TType? @object,
            Func<(TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)> args)
            where TType : class
        {
            if (IsNull(@object))
            {
                var (arg1, arg2, arg3, arg4) = args.Invoke();

                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
    }
}