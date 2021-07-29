using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class ParameterInfoExtensions
    {
        public static bool IsGqlTypeRequired(this ParameterInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaTypeRequiredAttribute>(true) != null;
        }

        public static bool IsGqlArrayRequired(this ParameterInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaArrayRequiredAttribute>(true) != null;
        }

        public static void AssertParameterTypeIsNotNullable(this ParameterInfo parameterInfo)
        {
            var methodInfo = parameterInfo.Member as MethodInfo;

            if (parameterInfo.ParameterType.IsGenericNullableType())
            {
                var arguments = string.Join(", ", methodInfo.GetParameters().Select(parameter => $"{parameter.ParameterType.GetFriendlyName()} {parameter.Name}"));

                throw new SchemaException($"The argument '{parameterInfo.Name}' on method '{methodInfo.DeclaringType!.FullName}.{methodInfo.Name}({arguments})' " +
                                          $"cannot be nullable. The presence of {nameof(SchemaTypeRequiredAttribute)} is used to declare a parameter is required, " +
                                           "and its absence makes it optional.");
            }
        }
    }
}