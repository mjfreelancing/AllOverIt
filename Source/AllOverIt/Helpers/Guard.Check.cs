using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Helpers
{
    public static partial class Guard
    {
        public static void CheckNotNull<TType>(this TType @object, string name, string errorMessage = default)
            where TType : class
        {
            _ = @object ?? ThrowInvalidOperationException<TType>(name, errorMessage ?? "Value cannot be null");
        }

        public static void CheckNotNullOrEmpty<TType>(this IEnumerable<TType> @object, string name, string errorMessage = default)
            where TType : class
        {
            CheckNotNull(@object, name, errorMessage);
            CheckNotEmpty(@object, name, errorMessage);
        }

        public static void CheckNotEmpty<TType>(this IEnumerable<TType> @object, string name, string errorMessage = default)
            where TType : class
        {
            if (@object != null && !@object.Any())
            {
                ThrowInvalidOperationException<IEnumerable<TType>>(name, errorMessage ?? "Value cannot be empty");
            }
        }

        public static void CheckNotNullOrEmpty(this string @object, string name, string errorMessage = default)
        {
            CheckNotNull(@object, name, errorMessage);
            CheckNotEmpty(@object, name, errorMessage);
        }

        public static void CheckNotEmpty(this string @object, string name, string errorMessage = default)
        {
            if (@object != null && string.IsNullOrWhiteSpace(@object))
            {
                ThrowInvalidOperationException(name, errorMessage ?? "Value cannot be empty");
            }
        }
    }
}