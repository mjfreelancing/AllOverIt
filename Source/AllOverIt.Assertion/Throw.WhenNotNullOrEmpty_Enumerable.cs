namespace AllOverIt.Assertion
{
    public static partial class Throw<TException> where TException : Exception
    {
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        public static void WhenNotNullOrEmpty<TType>(IEnumerable<TType>? @object)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1>(IEnumerable<TType>? @object, TExceptionArg1 arg1)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">A Func that returns the argument to be provided to the exception constructor.</param>
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1>(IEnumerable<TType>? @object, Func<TExceptionArg1> arg1)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                ThrowException(arg1.Invoke());
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1, TExceptionArg2>(IEnumerable<TType>? @object, TExceptionArg1 arg1, TExceptionArg2 arg2)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="args">A Func that returns the two arguments to be provided to the exception constructor.</param>
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1, TExceptionArg2>(IEnumerable<TType>? @object, Func<(TExceptionArg1 arg1, TExceptionArg2 arg2)> args)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                var (arg1, arg2) = args.Invoke();

                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(IEnumerable<TType>? @object, TExceptionArg1 arg1,
            TExceptionArg2 arg2, TExceptionArg3 arg3)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="args">A Func that returns the three arguments to be provided to the exception constructor.</param>
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3>(IEnumerable<TType>? @object,
            Func<(TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)> args)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                var (arg1, arg2, arg3) = args.Invoke();

                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
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
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(IEnumerable<TType>? @object,
            TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="object"/> is not <see langword="null"/> or empty.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="object">The object instance.</param>
        /// <param name="args">A Func that returns the four arguments to be provided to the exception constructor.</param>
        public static void WhenNotNullOrEmpty<TType, TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>(IEnumerable<TType>? @object,
            Func<(TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)> args)
            where TType : class
        {
            if (!IsNullOrEmpty(@object))
            {
                var (arg1, arg2, arg3, arg4) = args.Invoke();

                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
    }
}