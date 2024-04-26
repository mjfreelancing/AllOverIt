namespace AllOverIt.Validation
{
    internal static class ValidationTypeHelper
    {
        public static Type GetModelType(Type validatorType)
        {
            var baseType = validatorType.BaseType;

            if (!baseType.IsGenericType)
            {
                return GetModelType(baseType);
            }

            return baseType.GetGenericArguments()[0];
        }
    }
}