using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Helpers
{
    public static partial class Guard
    {
        public static TType InvalidWhenNull<TType>(this TType @object, string errorMessage = default)
            where TType : class
        {
            return @object ?? ThrowInvalidOperationException<TType>(errorMessage ?? "Value cannot be null");
        }

        public static IEnumerable<TType> InvalidWhenNullOrEmpty<TType>(this IEnumerable<TType> @object, string errorMessage = default)
            where TType : class
        {
            InvalidWhenNull(@object, errorMessage);
            return InvalidWhenEmpty(@object, errorMessage);
        }

        public static IEnumerable<TType> InvalidWhenEmpty<TType>(this IEnumerable<TType> @object, string errorMessage = default)
            where TType : class
        {
            if (@object != null && !@object.Any())
            {
                ThrowInvalidOperationException<IEnumerable<TType>>(errorMessage ?? "Value cannot be empty");
            }

            return @object;
        }

        public static string InvalidWhenNullOrEmpty(this string @object, string errorMessage = default)
        {
            CheckNotNull(@object, errorMessage);
            return InvalidWhenEmpty(@object, errorMessage);
        }

        public static string InvalidWhenEmpty(this string @object, string errorMessage = default)
        {
            if (@object != null && string.IsNullOrWhiteSpace(@object))
            {
                ThrowInvalidOperationException<string>(errorMessage ?? "Value cannot be empty");
            }

            return @object;
        }
    }
}