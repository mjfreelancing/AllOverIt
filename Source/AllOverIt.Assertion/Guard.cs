using System;

namespace AllOverIt.Assertion
{
    /// <summary>Provides a number of extensions that enable method pre-condition checking.</summary>
    public static partial class Guard
    {
        private static void ThrowArgumentNullException(string name, string errorMessage)
        {
            ThrowArgumentNullException<string>(name, errorMessage);
        }

        private static void ThrowArgumentNullException<TType>(string name, string errorMessage)
        {
            if (errorMessage == default)
            {
                throw new ArgumentNullException(name);
            }

            throw new ArgumentNullException(name, errorMessage);
        }

        private static void ThrowInvalidOperationException(string name, string errorMessage)
        {
            ThrowInvalidOperationException<string>(name, errorMessage);
        }

        private static void ThrowInvalidOperationException<TType>(string name, string errorMessage)
        {
            throw new InvalidOperationException($"{errorMessage} ({name})");
        }
    }
}
