using System;

namespace AllOverIt.Assertion
{
    /// <summary>Provides a number of extensions that enable method pre-condition checking.</summary>
    public static partial class Guard
    {
        private static Exception CreateArgumentNullException(string name, string? errorMessage)
        {
            if (errorMessage is null)
            {
                throw new ArgumentNullException(name);
            }

            throw new ArgumentNullException(name, errorMessage);
        }

        private static Exception CreateInvalidOperationException(string name, string errorMessage)
        {
            throw new InvalidOperationException($"{errorMessage} ({name})");
        }
    }
}
