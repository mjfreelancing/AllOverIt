using AllOverIt.Extensions;
using AllOverIt.Validation.Exceptions;

namespace AllOverIt.Validation
{
    internal static class ValidationTypeHelper
    {
        public static Type GetModelType(Type validatorType)
        {
            if (!validatorType.IsDerivedFrom(typeof(ValidatorBase<>)))
            {
                throw new ValidationRegistryException($"The type '{validatorType.GetFriendlyName()}' is not a validator.");
            }

            var baseType = validatorType.BaseType!;

            if (!baseType.IsGenericType)
            {
                return GetModelType(baseType);
            }

            return baseType.GetGenericArguments()[0];
        }
    }
}