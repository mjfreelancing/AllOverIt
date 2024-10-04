using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Assertion
{
    /// <summary>Static helper class that will throw a specified exception type when a given criteria is satisfied.</summary>
    /// <typeparam name="TException">The exception type to be thrown.</typeparam>
    public static partial class Throw<TException> where TException : Exception
    {
        #region ThrowException

        [DoesNotReturn]
        private static void ThrowException()
        {
            throw (Exception)Activator.CreateInstance<TException>()!;
        }

        [DoesNotReturn]
        private static void ThrowException<TArg1>(TArg1 arg1)
        {
            throw (Exception)Activator.CreateInstance(typeof(TException), [arg1])!;
        }

        [DoesNotReturn]
        private static void ThrowException<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            throw (Exception)Activator.CreateInstance(typeof(TException), [arg1, arg2])!;
        }

        [DoesNotReturn]
        private static void ThrowException<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            throw (Exception)Activator.CreateInstance(typeof(TException), [arg1, arg2, arg3])!;
        }

        [DoesNotReturn]
        private static void ThrowException<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            throw (Exception)Activator.CreateInstance(typeof(TException), [arg1, arg2, arg3, arg4])!;
        }

        #endregion

        private static bool IsNull<TType>([NotNullWhen(false)] TType? @object)
        {
            return @object is null;
        }

        private static bool IsNotNull<TType>([NotNullWhen(true)] TType @object)
        {
            // This is more efficient that !IsNull()
            return @object is not null;
        }

        private static bool IsNullOrEmpty([NotNullWhen(false)] string? @object)
        {
            return string.IsNullOrWhiteSpace(@object);
        }

        private static bool IsNullOrEmpty<TType>([NotNullWhen(false)] IEnumerable<TType>? @object)
        {
            return @object is null || !@object.Any();
        }
    }
}