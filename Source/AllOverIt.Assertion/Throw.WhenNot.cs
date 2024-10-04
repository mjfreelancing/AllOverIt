using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Assertion
{
    /// <summary>Static helper class that will throw a specified exception type when a given criteria is satisfied.</summary>
    /// <typeparam name="TException">The exception type to be thrown.</typeparam>
    public static partial class Throw<TException> where TException : Exception
    {
        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <param name="condition">The predicate condition.</param>
        public static void WhenNot(bool condition)
        {
            if (!condition)
            {
                ThrowException();
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1>([DoesNotReturnIf(false)] bool condition, TExceptionArg1 arg1)
        {
            if (!condition)
            {
                ThrowException(arg1);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">A Func that returns the argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1>([DoesNotReturnIf(false)] bool condition, Func<TExceptionArg1> arg1)
        {
            if (!condition)
            {
                ThrowException(arg1.Invoke());
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2>([DoesNotReturnIf(false)] bool condition, TExceptionArg1 arg1, TExceptionArg2 arg2)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="args">A Func that returns the two arguments to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2>([DoesNotReturnIf(false)] bool condition, Func<(TExceptionArg1 arg1, TExceptionArg2 arg2)> args)
        {
            if (!condition)
            {
                var (arg1, arg2) = args.Invoke();

                ThrowException(arg1, arg2);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3>([DoesNotReturnIf(false)] bool condition, TExceptionArg1 arg1,
            TExceptionArg2 arg2, TExceptionArg3 arg3)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="args">A Func that returns the three arguments to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3>([DoesNotReturnIf(false)] bool condition,
            Func<(TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3)> args)
        {
            if (!condition)
            {
                var (arg1, arg2, arg3) = args.Invoke();

                ThrowException(arg1, arg2, arg3);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="arg1">The first argument to be provided to the exception constructor.</param>
        /// <param name="arg2">The second argument to be provided to the exception constructor.</param>
        /// <param name="arg3">The third argument to be provided to the exception constructor.</param>
        /// <param name="arg4">The fourth argument to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>([DoesNotReturnIf(false)] bool condition,
            TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)
        {
            if (!condition)
            {
                ThrowException(arg1, arg2, arg3, arg4);
            }
        }

        /// <summary>Throws a <typeparamref name="TException"/> when the <paramref name="condition"/> is <see langword="False"/>.</summary>
        /// <typeparam name="TExceptionArg1">The first exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg2">The second exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg3">The third exception argument type.</typeparam>
        /// <typeparam name="TExceptionArg4">The fourth exception argument type.</typeparam>
        /// <param name="condition">The predicate condition.</param>
        /// <param name="args">A Func that returns the four arguments to be provided to the exception constructor.</param>
        public static void WhenNot<TExceptionArg1, TExceptionArg2, TExceptionArg3, TExceptionArg4>([DoesNotReturnIf(false)] bool condition,
            Func<(TExceptionArg1 arg1, TExceptionArg2 arg2, TExceptionArg3 arg3, TExceptionArg4 arg4)> args)
        {
            if (!condition)
            {
                var (arg1, arg2, arg3, arg4) = args.Invoke();

                ThrowException(arg1, arg2, arg3, arg4);
            }
        }
    }
}