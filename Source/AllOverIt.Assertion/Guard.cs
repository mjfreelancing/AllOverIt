using System;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Assertion
{
    /// <summary>Provides a number of extensions that enable method pre-condition checking.</summary>
    public static partial class Guard
    {
        [DoesNotReturn]
        private static void ThrowEmptyArgumentException(string name, string? errorMessage)
        {
            throw new ArgumentException(errorMessage ?? "The argument cannot be empty.", name);
        }

        [DoesNotReturn]
        private static void ThrowArgumentNullException(string name, string? errorMessage)
        {
            if (errorMessage is null)
            {
                throw new ArgumentNullException(name);
            }

            throw new ArgumentNullException(name, errorMessage);
        }

        [DoesNotReturn]
        private static void ThrowInvalidOperationException(string name, string errorMessage)
        {
            throw new InvalidOperationException($"{errorMessage} ({name})");
        }
    }
}
